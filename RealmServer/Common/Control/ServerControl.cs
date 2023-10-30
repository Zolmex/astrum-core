using Common.Database;
using Common.Resources.World;
using Common.Utilities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Control
{
    public static class ServerControl
    {
        private const int KEEP_ALIVE_MS = 2000;
        private const int TIMEOUT_MS = 10000;

        private static ILogger _log = new Logger(typeof(ServerControl));
        private static ConcurrentDictionary<string, int> _serversTimeout;
        private static Thread _loopThread;
        private static string _instanceID;
        private static ISubscriber _sub;

        public static EventHandler<ControlMessage<ServerMember>> MemberEntered; // Use later
        public static EventHandler<ControlMessage<ServerMember>> MemberLeft;
        public static EventHandler<ControlMessage<ServerMember>> MemberAlive;
        public static ConcurrentDictionary<string, ServerMember> Members;
        public static ServerMember Host;

        public static void Connect(MemberType type, string memberName, ServerInfo serverInfo = null)
        {
            _serversTimeout = new ConcurrentDictionary<string, int>();
            _instanceID = Guid.NewGuid().ToString(); // Setup our Host object
            _sub = RedisClient.Sub;

            Host = new ServerMember()
            {
                Type = type,
                Name = memberName,
                InstanceID = _instanceID,
                ServerInfo = serverInfo
            };

            Members = new ConcurrentDictionary<string, ServerMember>(); // Setup our dictionaries responsible of keeping track of current active servers
            Members.TryAdd(Host.InstanceID, Host);

            MemberEntered += OnServerEntered; // Setup listeners to know if a server has entered, left or is still active
            MemberLeft += OnServerLeft;
            MemberAlive += OnServerAlive;

            _loopThread = new Thread(TickLoop) { Priority = ThreadPriority.Lowest };
            _loopThread.Start();

            Subscribe(ControlChannel.MemberEnter, MemberEntered);
            Subscribe(ControlChannel.MemberLeave, MemberLeft);
            Subscribe(ControlChannel.KeepAlive, MemberAlive);
            Publish(ControlChannel.MemberEnter, _instanceID, null, Host);
        }

        public static void SetLogger(ILogger logger)
        {
            _log = logger;
        }

        private static void TickLoop()
        {
            while (true)
            {
                foreach (var id in _serversTimeout.Keys)
                    _serversTimeout[id] += KEEP_ALIVE_MS;

                var remove = new List<string>();
                foreach (var kvp in _serversTimeout)
                {
                    if (kvp.Value >= TIMEOUT_MS)
                    {
                        Members.TryRemove(kvp.Key, out var host);
                        if (host != null)
                            MemberLeft.Invoke(null, new ControlMessage<ServerMember>()
                            {
                                InstanceID = host.InstanceID,
                                TargetID = null,
                                Content = host
                            });
                        remove.Add(kvp.Key);
                    }
                }

                foreach (var k in remove)
                    _serversTimeout.Remove(k, out _);

                Publish(ControlChannel.KeepAlive, _instanceID, null, Host);
                Thread.Sleep(KEEP_ALIVE_MS);
            }
        }

        public static void Publish<T>(ControlChannel channel, string senderId, string targetId, T content)
        {
            var message = new ControlMessage<T>()
            {
                InstanceID = senderId,
                TargetID = targetId,
                Content = content
            };
            _sub.PublishAsync(channel.ToString(), JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);
        }

        public static void Subscribe<T>(ControlChannel channel, EventHandler<ControlMessage<T>> callback)
        {
            _sub.SubscribeAsync(channel.ToString(), (c, v) =>
            {
                var message = JsonConvert.DeserializeObject<ControlMessage<T>>(Encoding.UTF8.GetString(v));
                if (message.InstanceID != _instanceID &&
                    (message.TargetID == null || message.TargetID == _instanceID))
                    callback.Invoke(null, message);
            }, CommandFlags.FireAndForget);
        }

        private static void OnServerEntered(object sender, ControlMessage<ServerMember> message)
        {
            if (Members.TryAdd(message.InstanceID, message.Content))
            {
                Publish(ControlChannel.MemberEnter, Host.InstanceID, message.InstanceID, Host);

                _log.Info($"Member '{message.Content.Name}' has joined the network.");
            }
        }

        private static void OnServerLeft(object sender, ControlMessage<ServerMember> message)
        {
            Members.TryRemove(message.InstanceID, out _);

            _log.Info($"Member '{message.Content.Name}' has left the network.");
        }

        private static void OnServerAlive(object sender, ControlMessage<ServerMember> message)
        {
            _serversTimeout[message.InstanceID] = 0;
        }
    }
}
