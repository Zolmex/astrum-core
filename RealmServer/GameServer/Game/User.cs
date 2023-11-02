using GameServer.Game.Net;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using Common.Utilities;
using Common.Database;
using Common.Resources.World;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using GameServer.Game.Logic.Entities;
using GameServer.Game.Logic;
using GameServer.Game.Worlds;

namespace GameServer.Game
{
    public enum ConnectionState
    {
        Disconnected, // Client is waiting to be used
        Connected, // A connection received is using this instance of NetClient
        Reconnecting, // Client is moving from a world instance to another
        Ready // Client is ready to handle in-game packets
    }

    public enum DisconnectReason
    {
        Unknown,
        UserDisconnect,
        NetworkError,
        Failure,
        Death
    }

    public class User : IIdentifiable
    {
        private static readonly Logger _log = new Logger(typeof(User));
        private static int _nextClientId;

        public int Id { get; set; }
        public NetworkHandler Network { get; }
        public GameInfo GameInfo { get; }

        public ConnectionState State { get; private set; }
        public DbAccount Account { get; private set; }
        public ClientRandom Random { get; private set; }

        private readonly object _disconnectLock = new object();

        public User()
        {
            Id = Interlocked.Increment(ref _nextClientId);
            Network = new NetworkHandler(this);
            GameInfo = new GameInfo(this);
        }

        public void Setup(string ip, Socket socket)
        {
            Network.Setup(ip, socket);
        }

        public void StartNetwork()
        {
            State = ConnectionState.Connected;
            Network.Start();
        }

        public void SetGameInfo(DbAccount acc, uint randomSeed, World world)
        {
            Account = acc;
            RealmManager.Users.SetAccId(this, acc.AccountId);

            Random = new ClientRandom(randomSeed);

            GameInfo.SetWorld(world);
        }

        public void Load(DbChar chr, World world)
        {
            GameInfo.Load(chr, world);

            SendPacket(PacketId.CREATESUCCESS, CreateSuccess.Write(this,
                GameInfo.Player.Id,
                chr.CharId));
            SendPacket(PacketId.ACCOUNTLIST, AccountList.Write(this,
                AccountList.Locked,
                Account.LockedIds ?? new int[0]));
            SendPacket(PacketId.ACCOUNTLIST, AccountList.Write(this,
                AccountList.Ignored,
                Account.IgnoredIds ?? new int[0]));

            State = ConnectionState.Ready;
        }

        private void Unload()
        {
            if (GameInfo.State != GameState.Playing) // We can only unload when we've loaded in the first place
                return;

            GameInfo.Unload();
        }

        public void Reset()
        {
            Network.Reset();
            GameInfo.Reset();

            Account = null;
            Random = null;
        }

        public void SendPacket(PacketId packetId, StreamWriteInfo writeInfo)
        {
            Network.Send(packetId, writeInfo);
        }

        public OutgoingPacket GetPacket(PacketId packetId)
        {
            return Network.GetPacket(packetId);
        }

        public async void SendFailure(int errorId = Failure.DEFAULT, string message = Failure.DEFAULT_MESSAGE)
        {
            Network.Send(PacketId.FAILURE, Failure.Write(this, errorId, message));

            await Task.Delay(1000);
            Disconnect(message, DisconnectReason.Failure);
        }

        public void Disconnect(string message = null, DisconnectReason reason = DisconnectReason.Unknown)
        {
            lock (_disconnectLock)
            {
                if (State == ConnectionState.Disconnected)
                    return;

                _log.Info($"Disconnecting user {Id} ({message}) (Reason:{reason})");

                State = ConnectionState.Disconnected;

                Unload();

                RealmManager.DisconnectUser(this);
                SocketServer.DisconnectUser(this);
            }
        }

        public void ReconnectTo(World world)
        {
            State = ConnectionState.Reconnecting;

            Unload(); // Begin reconnect process, kill player entity and set gamestate to idle

            SendPacket(PacketId.RECONNECT, Reconnect.Write(this,
                world.Id
                ));
        }
    }
}
