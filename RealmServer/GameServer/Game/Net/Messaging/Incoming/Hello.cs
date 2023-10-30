using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Text;
using StackExchange.Redis;
using Common.Database;
using GameServer.Game.Logic;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class Hello : IncomingPacket
    {
        public string BuildVersion { get; set; }
        public int GameId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MapJSONLength { get; set; }
        public string MapJSON { get; set; }

        public override PacketId ID => PacketId.HELLO;

        protected override void Read(NetworkReader rdr)
        {
            BuildVersion = rdr.ReadUTF();
            GameId = rdr.ReadInt32();
            Username = rdr.ReadUTF();
            Password = rdr.ReadUTF();
            MapJSONLength = rdr.ReadInt32();
            MapJSON = Encoding.UTF8.GetString(rdr.ReadBytes(MapJSONLength));
        }

        protected override void Handle(User user)
        {
            if (BuildVersion != GameServerConfig.Config.Version)
            {
                user.SendFailure(Failure.INCORRECT_VERSION, GameServerConfig.Config.Version);
                return;
            }

            var acc = DbClient.VerifyAccount(Username, Password, out var status);
            if (acc == null)
            {
                user.SendFailure(Failure.DEFAULT, status.GetDescription());
                return;
            }

            if (GameServerConfig.Config.AdminOnly && !acc.Admin)
            {
                user.SendFailure(Failure.DEFAULT, "Admin only server.");
                return;
            }

            var world = RealmManager.GetWorld(GameId);
            if (world == null)
            {
                user.SendFailure(Failure.DEFAULT, "Invalid target world.");
                return;
            }

            var seed = (uint)new Random().Next(1, int.MaxValue);
            user.SetGameInfo(acc, seed, GameId);

            user.SendPacket(PacketId.MAPINFO, MapInfo.Write(user,
                world.Map.Width,
                world.Map.Height,
                world.Config.Name,
                world.Config.DisplayName,
                seed,
                world.Config.Background,
                world.Config.AllowTeleport,
                world.Config.ShowDisplays));
        }

        public override string ToString()
        {
            var type = typeof(Hello);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }
            return ret;
        }
    }
}
