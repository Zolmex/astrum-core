using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Logic;
using GameServer.Game.Logic.Worlds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public struct RealmTime
    {
        public long TickCount;
        public long TotalElapsedMs;
        public int ElapsedMsDelta;
    }

    public static class RealmManager
    {
        private static readonly Logger _log = new Logger(typeof(RealmManager));

        public static RealmTime GlobalTime; // This field is updated every tick
        public static readonly List<RealmTime> TickHistory = new List<RealmTime>();

        public static readonly UserCollection Users = new UserCollection();
        public static readonly WorldCollection Worlds = new WorldCollection();

        public static readonly List<string> ActiveRealms = new List<string>();

        public static void Init()
        {
            PlayerCommand.Load();

            AddWorld(new Nexus());

            _log.Info("RealmManager initialized.");
        }

        public static void Run(int mspt)
        {
            var lagMs = (int)(mspt * 1.5);
            var sw = Stopwatch.StartNew();
            while (true)
            {
                Update();

                // This approach uses more CPU power but it's much more accurate than others (e.g Thread.Sleep, ManualResetEvent)
                if (sw.ElapsedMilliseconds < mspt)
                    continue;

                GlobalTime.ElapsedMsDelta = (int)sw.ElapsedMilliseconds;
                GlobalTime.TotalElapsedMs += sw.ElapsedMilliseconds;
                GlobalTime.TickCount++;
                TickHistory.Add(GlobalTime);
#if DEBUG
                if (GlobalTime.ElapsedMsDelta >= lagMs)
                    _log.Warn($"LAGGED | MsPT: {mspt} Elapsed: {GlobalTime.ElapsedMsDelta}");
#endif
                sw.Restart();

                foreach (var kvp in Worlds)
                    kvp.Value.Tick(GlobalTime);
            }
        }

        private static void Update()
        {
            Users.Update();
            Worlds.Update();

            foreach (var kvp in Worlds)
                kvp.Value.Update();
        }

        public static void ConnectUser(User user)
        {
            Users.Add(user);

            user.StartNetwork();
        }

        public static void DisconnectUser(User user)
        {
            Users.Remove(user.Id);
        }

        public static void AddWorld(World world)
        {
            Worlds.Add(world);
        }

        public static void RemoveWorld(int worldId)
        {
            Worlds.Remove(worldId);
        }

        public static World GetWorld(int worldId)
        {
            return Worlds.Get(worldId);
        }

        public static World GetWorld(string worldName)
        {
            return Worlds.Get(worldName);
        }

        public static void OnRealmAdded(string realmName)
        {
            ActiveRealms.Add(realmName);
            ChatManager.Announce($"A portal to {realmName} has been opened.");
        }
    }
}
