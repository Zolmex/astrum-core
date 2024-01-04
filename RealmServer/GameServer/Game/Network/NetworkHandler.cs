using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using Common.Database;
using System.Runtime.InteropServices.ObjectiveC;

namespace GameServer.Game.Network
{
    public class SocketReceiveState
    {
        public readonly NetworkReader Reader;

        public byte[] Buffer { get; set; } = new byte[NetworkHandler.BUFFER_SIZE];
        public PacketId PacketId { get; set; }
        public int PacketLength { get; set; }
        public int BytesAvailable { get; set; }

        public SocketReceiveState()
        {
            Reader = new NetworkReader(new MemoryStream(Buffer));
        }

        public int ReadRemainingBytes(int bytesNotRead)
        {
            if (PacketLength == 0)
            {
                if (BytesAvailable + bytesNotRead < 4)
                {
                    BytesAvailable += bytesNotRead;
                    return bytesNotRead;
                }

                PacketLength = Reader.ReadInt32();
            }

            if (BytesAvailable + bytesNotRead < PacketLength - 4)
            {
                BytesAvailable += bytesNotRead;
                return bytesNotRead;
            }

            PacketId = (PacketId)Reader.ReadByte(); // There are enough bytes to read the entire packet, so we read just the bytes we need for the current packet being read
            var count = PacketLength - BytesAvailable;

            BytesAvailable = PacketLength;
            return count;
        }

        public void EndRead()
        {
            try
            {
                if (BytesAvailable != 0)
                    System.Buffer.BlockCopy(Buffer, (int)Reader.BaseStream.Position, Buffer, 0, BytesAvailable);
                Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void EndPacketRead()
        {
            PacketLength = 0;
            BytesAvailable = 0;
            PacketId = PacketId.FAILURE;
        }

        public void Reset()
        {
            PacketLength = 0;
            BytesAvailable = 0;
            PacketId = PacketId.FAILURE;
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }
    }

    public class SocketSendState
    {
        public NetworkWriter Writer { get; private set; }
        public MemoryStream Stream { get; private set; }
        public byte[] SocketBuffer { get; private set; } = new byte[NetworkHandler.BUFFER_SIZE];
        public int LastValidIndex { get; private set; }

        public SocketSendState()
        {
            Stream = new MemoryStream();
            Writer = new NetworkWriter(Stream);
        }

        public int PacketBegin() // Returns the start of the packet bytes in the buffer. NEVER use this without locking the SocketSendState instance
        {
            var begin = (int)Stream.Position;
            Stream.Position += 5; // Leave 5 bytes for the header
            return begin;
        }

        public void PacketEnd(int begin, PacketId pkt)
        {
            var length = (int)Stream.Position - begin;
            Stream.Position -= length; // Write the header [length][id]
            Writer.Write(length);
            Writer.Write((byte)pkt);
            Stream.Position += length - 5; // Go to the next position after packet body to write the next packet

            if (Stream.Position < NetworkHandler.BUFFER_SIZE)
                LastValidIndex = (int)Stream.Position;

            //Logger.Debug($"WRITING {pkt} DATA TO BUFFER");
        }

        public void Reset()
        {
            lock (this)
            {
                LastValidIndex = 0;
                Stream.SetLength(0);
            }
        }
    }

    // Handles all of the network socket communication
    public class NetworkHandler
    {
        public const int BUFFER_SIZE = 131072; // 128 KB

        private static readonly Logger _log = new Logger(typeof(NetworkHandler));

        public string IP { get; private set; }
        public User User { get; private set; }
        public Socket Socket { get; private set; }
        public SocketSendState SendState { get; private set; }

        private readonly Dictionary<PacketId, OutgoingPacket> _outgoing;
        private readonly Dictionary<PacketId, IncomingPacket> _incoming;
        private readonly SocketReceiveState _receiveState;
        private readonly SocketAsyncEventArgs _receive;
        private readonly SocketAsyncEventArgs _send;

        public NetworkHandler(User user)
        {
            User = user;

            _receive = new SocketAsyncEventArgs();
            _receiveState = new SocketReceiveState();
            _send = new SocketAsyncEventArgs();
            SendState = new SocketSendState();

            // Setup our SAEA objects
            _receive.SetBuffer(_receiveState.Buffer, 0, BUFFER_SIZE);
            _receive.Completed += ProcessReceive;
            _send.SetBuffer(SendState.SocketBuffer, 0, BUFFER_SIZE);
            _send.Completed += ProcessSend;

            _incoming = Packet.LoadIncoming(this);
            _outgoing = Packet.LoadOutgoing(this);
        }

        // Reset this instance's values for a possible future connection
        public void Reset()
        {
            IP = null;

            _receiveState.Reset();
            SendState.Reset();
        }

        public void Setup(string ip, Socket socket)
        {
            IP = ip;
            Socket = socket;
            Socket.NoDelay = true;
        }

        public void Start()
        {
            StartSend();
            StartReceive();
        }

        public async void StartSend()
        {
            while (SendState.Stream.Position == 0)
            {
                await Task.Delay(1);

                if (User.State == ConnectionState.Disconnected || !Socket.Connected)
                {
                    User.Disconnect(reason: DisconnectReason.Unknown);
                    return;
                }
            }

            int count;
            lock (SendState)
            {
                count = SendState.LastValidIndex; // Gets the position of the last packet that fits within the buffer size

                var streamBuffer = SendState.Stream.GetBuffer();
                Buffer.BlockCopy(streamBuffer, 0, SendState.SocketBuffer, 0, count);

                SendState.Stream.Position = 0;
            }

            //_log.Debug($"SENDING {count} bytes TO {User.Id}");

            // ASYNC SEND
            _send.SetBuffer(0, count);

            try
            {
                if (Socket != null && !Socket.SendAsync(_send))
                    ProcessSend(null, _send);
            }
            catch { }
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs args)
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            var error = args.SocketError;
            if (error != SocketError.Success && error != SocketError.IOPending)
            {
                string msg = null;
                if (error != SocketError.ConnectionReset)
                    msg = $"Send SocketError.{error}";
                User.Disconnect(msg, DisconnectReason.NetworkError);
                return;
            }

            StartSend();
        }

        private void StartReceive()
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            // Some exceptions from the SAEA.SetBuffer method and from the Socket.ReceiveAsync() method
            // can't be avoided in certain situations, so a try-catch block is required.
            try
            {
                _receive.SetBuffer(_receiveState.BytesAvailable, BUFFER_SIZE - _receiveState.BytesAvailable);

                if (!Socket.ReceiveAsync(_receive))
                    ProcessReceive(null, _receive);
            }
            catch { }
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs args)
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            // Check for any errors during the operation
            var error = args.SocketError;
            if (error != SocketError.Success && error != SocketError.IOPending)
            {
                string msg = null;
                if (error != SocketError.ConnectionReset)
                    msg = $"Receive SocketError.{error}";
                User.Disconnect(msg, DisconnectReason.NetworkError);
                return;
            }

            var bytesNotRead = args.BytesTransferred;

            // When using ReceiveAsync, this value is set to 0 when the connection is terminated by the user
            if (bytesNotRead == 0)
            {
                User.Disconnect(reason: DisconnectReason.UserDisconnect);
                return;
            }

            // Start reading bytes from the previous point
            while (bytesNotRead > 0)
            {
                var read = _receiveState.ReadRemainingBytes(bytesNotRead);

                // Policy file check
                if (_receiveState.PacketLength == 1014001516)
                {
                    SendPolicyFile();
                    break;
                }

                bytesNotRead -= read;
                //_log.Debug($"RECEIVING {read} BYTES FROM {User.Id} L:{_receiveState.PacketLength} B:{_receiveState.BytesAvailable}");

                // If the entire packet is read, process it
                if (_receiveState.PacketLength != 0 && _receiveState.BytesAvailable == _receiveState.PacketLength)
                {
                    var packetId = _receiveState.PacketId;
                    if (_incoming.ContainsKey(packetId))
                    {
                        try
                        {
                            //_log.Debug($"RECEIVING {packetId} ({_receiveState.BytesAvailable} bytes)");
                            _incoming[packetId].ReadBody(User, _receiveState.Reader);
                        }
                        catch (Exception e)
                        {
                            _log.Error(e);
                            User.SendFailure(Failure.DEFAULT, $"Error processing packet {packetId}: {e.Message}");
                        }
                    }
                    else // This means the reader's position is still at the packetid, we need to move that to the end of the packet
                    {
                        var count = _receiveState.PacketLength - 5; // Subtract 4 bytes (length) + 1 byte (packetid)
                        _receiveState.Reader.BaseStream.Seek(count, SeekOrigin.Current);
                    }

                    // Reset the packet reading process
                    _receiveState.EndPacketRead();
                }
            }

            _receiveState.EndRead();

            StartReceive();
        }

        //public void Send(PacketId id, StreamWriteInfo writeInfo)
        //{
        //    if (User.State == ConnectionState.Disconnected)
        //        return;

        //    var pkt = _outgoing[id];
        //    pkt.AddPending(writeInfo);
        //}

        private void SendPolicyFile()
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
                return;

            try
            {
                var wtr = new NetworkWriter(new NetworkStream(Socket));
                wtr.WriteNullTerminatedString(
                        @"<cross-domain-policy>" +
                        @"<allow-access-from domain=""*"" to-ports=""*"" />" +
                        @"</cross-domain-policy>");
                wtr.Write((byte)'\r');
                wtr.Write((byte)'\n');
                wtr.Close();
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
            }
        }
    }
}
