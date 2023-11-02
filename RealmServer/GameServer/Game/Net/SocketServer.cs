using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Net.Messaging.Outgoing;
using GameServer.Game.Net.Messaging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Common.Resources.Config;
using GameServer.Game.Collections;

namespace GameServer.Game.Net
{
    // TCP socket server
    public static class SocketServer
    {

        private static readonly Logger _log = new Logger(typeof(SocketServer));
        private static ConcurrentFactory<User> _userFactory;
        private static UserCollection _users;
        private static Dictionary<string, int> _ips;
        private static Socket _socket;

        // Start accepting connections
        public static void Start(int port, int maxConnections)
        {
            _ips = new Dictionary<string, int>(); // Used to keep track of how many clients per IP address
            _userFactory = new ConcurrentFactory<User>(maxConnections); // Used to prevent memory leaks
            _users = new UserCollection();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(3000); // Backlog is the max number of pending connections

            StartAccept();
            new Thread(SendPackets).Start();
        }

        private static void StartAccept()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += ProcessAccept;

            try
            {
                // Socket async methods return false when they complete synchronously, meaning that the Completed
                // event won't be invoked, so we have to manually call the callback ourselves
                if (!_socket.AcceptAsync(args))
                    ProcessAccept(null, args);
            }
            catch { }
        }

        private static void ProcessAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success) // If error, recycle and continue
            {
                args.Dispose();
                StartAccept();
                return;
            }

            var skt = args.AcceptSocket;
            var ip = ((IPEndPoint)skt.RemoteEndPoint).Address.ToString();

            var maxClientsReached = false;
            var ipConnected = _ips.TryGetValue(ip, out int clientCount);
            if (ipConnected && clientCount >= GameServerConfig.Config.MaxClientsPerIP)
                maxClientsReached = true;

            if (ip == null || maxClientsReached)
            {
                args.Dispose();
                StartAccept();
                return;
            }

            if (ipConnected)
                _ips[ip] += 1;
            else
                _ips.TryAdd(ip, 1);

            _log.Debug($"Received client @ {ip}");

            var user = _userFactory.Pop(); // Give the connection a NetClient instance to communicate with
            user.Setup(ip, skt);
            _users.Add(user);

            RealmManager.ConnectUser(user);

            // Recycle the SAEA object
            args.Dispose();

            StartAccept();
        }

        public static void DisconnectUser(User user)
        {
            // Terminate connection with the Socket
            try
            {
                user.Network.Socket.Shutdown(SocketShutdown.Both);
                user.Network.Socket.Close();
            }
            catch { }

            _users.Remove(user.Id);
            _ips.Remove(user.Network.IP);

            // Recycle client instance
            user.Reset();

            _userFactory.Push(user);
        }

        private static void SendPackets()
        {
            while (true)
            {
                _users.Update();
                foreach (var kvp in _users)
                {
                    var user = kvp.Value;
                    var network = user.Network;
                    var args = network.GetSendArgs();
                    var state = (SocketSendState)args.UserToken;
                    foreach (var packetKvp in user.Network.Outgoing)
                    {
                        var packet = packetKvp.Value;
                        if (packet.PendingCount == 0)
                            continue;

                        // If all of the bytes written were sent, or if there were no bytes written at all, write the pending packets to the buffer
                        if (state.BytesAvailable <= 0)
                        {
                            var bytes = packet.Write(state.Buffer, state.BytesAvailable);
                            if (bytes == 0) // Returns 0 when writing to the buffer failed (not enough space)
                                continue;
                            //_log.Debug($"SENDING {packet.ID} TO {state.Handler.User.Id}");
                            state.BytesAvailable += bytes;
                        }

                        try
                        {
                            args.SetBuffer(0, state.BytesAvailable);

                            if (!state.Socket.SendAsync(args))
                                ProcessSend(null, args);
                        }
                        catch { }
                    }
                }
            }
        }

        private static void ProcessSend(object sender, SocketAsyncEventArgs args)
        {
            var state = (SocketSendState)args.UserToken;
            var network = state.Handler;

            if (network.User.State == ConnectionState.Disconnected || !state.Socket.Connected)
            {
                network.User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            var error = args.SocketError;
            if (error != SocketError.Success && error != SocketError.IOPending)
            {
                string msg = null;
                if (error != SocketError.ConnectionReset)
                    msg = $"Send SocketError.{error}";
                network.User.Disconnect(msg, DisconnectReason.NetworkError);
                return;
            }

            state.BytesAvailable -= args.BytesTransferred; // Substract the amount of bytes that were sent
            if (state.BytesAvailable <= 0)
                state.Reset();
        }
    }
}
