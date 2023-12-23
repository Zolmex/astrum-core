using GameServer.Game.Network;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using Common.Utilities;
using Common.Database;
using Common.Resources.World;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using GameServer.Game.Collections;
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
            State = ConnectionState.Ready;

            GameInfo.Load(chr, world);

            CreateSuccess.Write(Network,
                GameInfo.Player.Id,
                chr.CharId);
            AccountList.Write(Network,
                AccountList.Locked,
                Account.LockedIds ?? new int[0]);
            AccountList.Write(Network,
                AccountList.Ignored,
                Account.IgnoredIds ?? new int[0]);
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

        public async void SendFailure(int errorId = Failure.DEFAULT, string message = Failure.DEFAULT_MESSAGE)
        {
            Failure.Write(Network, errorId, message);

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

            Reconnect.Write(Network,
                world.Id
                );
        }
    }
}
