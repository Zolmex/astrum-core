using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Net
{
    public class SocketReceiveState
    {
        // Total receive buffer bytes
        public byte[] Buffer { get; set; } = new byte[NetworkHandler.BUFFER_SIZE];
        // These 3 next properties contribute to the process explained above.
        public PacketId PacketId { get; set; }
        public int PacketLength { get; set; }
        public int TotalBytesRead { get; set; }
        public int PacketBytesRead { get; set; }

        public int ReadReceivedBytes(int bytesReceived)
        {
            if (PacketLength == 0)
                return ReadNewPacket(bytesReceived);

            // Continue reading the rest of the packet
            var count = Math.Min(Math.Min(bytesReceived, PacketLength - PacketBytesRead), Buffer.Length - TotalBytesRead);
            PacketBytesRead += count;
            TotalBytesRead += count;
            return count;
        }

        private int ReadNewPacket(int bytesReceived)
        {
            // Since we are reading a new packet, we read the next 4 bytes, from the current position
            PacketLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Buffer, TotalBytesRead));
            PacketId = (PacketId)Buffer[TotalBytesRead + 4];

            if (PacketLength == 0) // Can occur, take care of this situation
                return 0;

            return ReadReceivedBytes(bytesReceived);
        }

        public void EndRead()
        {
            if (PacketLength != 0)
            {
                System.Buffer.BlockCopy(Buffer, TotalBytesRead - PacketBytesRead, Buffer, 0, PacketBytesRead);
                TotalBytesRead = PacketBytesRead;
                return;
            }

            PacketLength = 0;
            TotalBytesRead = 0;
            PacketBytesRead = 0;
            PacketId = PacketId.FAILURE;
            if (Buffer != null)
                Array.Clear(Buffer, 0, Buffer.Length);
        }

        public void EndPacketRead()
        {
            PacketLength = 0;
            PacketBytesRead = 0;
            PacketId = PacketId.FAILURE;
        }
    }

    public class SocketSendState
    {
        public NetworkHandler Handler { get; set; }
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; } = new byte[NetworkHandler.BUFFER_SIZE];
        // We store the bytes that were written, and substract from it the amount of bytes sent
        public int BytesAvailable { get; set; }

        public void Reset()
        {
            BytesAvailable = 0;
            if (Buffer != null)
                Array.Clear(Buffer, 0, Buffer.Length);
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
        public SocketReceiveState ReceiveState { get; private set; }
        public SocketSendState SendState { get; private set; }

        public readonly Dictionary<PacketId, OutgoingPacket> Outgoing;
        private readonly Dictionary<PacketId, IncomingPacket> _incomingPackets;
        private readonly SocketAsyncEventArgs _receive;
        private readonly SocketAsyncEventArgs _send;

        public NetworkHandler(User user)
        {
            User = user;

            _receive = new SocketAsyncEventArgs();
            _send = new SocketAsyncEventArgs();
            ReceiveState = new SocketReceiveState();
            SendState = new SocketSendState();
            SendState.Handler = this;

            _receive.UserToken = ReceiveState;
            _send.UserToken = SendState;

            _incomingPackets = Packet.LoadIncoming(this);
            Outgoing = Packet.LoadOutgoing(this);
        }

        // Reset this instance's values for a possible future connection
        public void Reset()
        {
            IP = null;
            Socket = null;

            ReceiveState.EndRead();
            SendState.Reset();
            SendState.Socket = null;

            _receive.Completed -= ProcessReceive;
        }

        public void Setup(string ip, Socket socket)
        {
            IP = ip;
            Socket = socket;
            Socket.NoDelay = true;
            SendState.Socket = socket;

            // Setup our SAEA objects
            _send.SetBuffer(SendState.Buffer, 0, BUFFER_SIZE); // Send object is managed by SocketServer
            _receive.SetBuffer(ReceiveState.Buffer, 0, BUFFER_SIZE);
            _receive.Completed += ProcessReceive;
        }

        public void Start()
        {
            StartReceive(_receive);
        }

        public SocketAsyncEventArgs GetSendArgs() // I hate this just as much as you do
        {
            return _send;
        }

        private void StartReceive(SocketAsyncEventArgs args)
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
                args.SetBuffer(ReceiveState.PacketBytesRead, BUFFER_SIZE - ReceiveState.PacketBytesRead);

                if (!Socket.ReceiveAsync(args))
                    ProcessReceive(null, args);
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

            var bytesReceived = args.BytesTransferred;

            // When using ReceiveAsync, this value is set to 0 when the connection is terminated by the user
            if (bytesReceived == 0)
            {
                User.Disconnect(reason: DisconnectReason.UserDisconnect);
                return;
            }

            // Start reading bytes from the previous point
            var state = (SocketReceiveState)args.UserToken;
            while (bytesReceived > 0)
            {
                var read = state.ReadReceivedBytes(args.BytesTransferred);

                // Policy file check
                if (state.PacketLength == 1014001516)
                {
                    SendPolicyFile();
                    break;
                }

                bytesReceived -= read;

                // If the entire packet is read, process it
                if (state.PacketBytesRead >= state.PacketLength)
                {
                    var packetId = state.PacketId;
                    if (_incomingPackets.ContainsKey(packetId))
                    {
                        try
                        {
                            //_log.Debug($"RECEIVING {packetId}");
                            _incomingPackets[packetId].ReadBody(User, state.Buffer, state.TotalBytesRead - state.PacketBytesRead + 5, state.PacketBytesRead - 5);
                        }
                        catch (Exception e)
                        {
                            _log.Error(e);
                            User.SendFailure(Failure.DEFAULT, $"Error processing packet {packetId}: {e.Message}");
                        }
                    }

                    // Reset the packet reading process
                    state.EndPacketRead();
                }
            }

            state.EndRead();

            StartReceive(args);
        }

        private void SendPolicyFile()
        {
            if (Socket == null)
                return;

            try
            {
                var s = new NetworkStream(Socket);
                var wtr = new NetworkWriter(s);
                wtr.WriteNullTerminatedString(
                    @"<cross-domain-policy>" +
                    @"<allow-access-from domain=""*"" to-ports=""*"" />" +
                    @"</cross-domain-policy>");
                wtr.Write((byte)'\r');
                wtr.Write((byte)'\n');
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
            }
        }

        public void Send(PacketId id, StreamWriteInfo writeInfo)
        {
            if (User.State == ConnectionState.Disconnected)
                return;

            var pkt = Outgoing[id];
            pkt.AddPending(writeInfo);
        }

        public OutgoingPacket GetPacket(PacketId packetId)
        {
            return Outgoing[packetId];
        }
    }
}
