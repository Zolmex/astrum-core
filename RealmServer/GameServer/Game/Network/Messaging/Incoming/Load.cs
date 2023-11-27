using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Linq;
using Common.Database;
using StackExchange.Redis;

namespace GameServer.Game.Network.Messaging.Incoming
{
    public class Load : IncomingPacket
    {
        public int CharId { get; set; }

        public override PacketId ID => PacketId.LOAD;

        protected override void Read(NetworkReader rdr)
        {
            CharId = rdr.ReadInt32();
        }

        protected override void Handle(User user)
        {
            var chr = user.GameInfo.Char;
            if (user.State != ConnectionState.Reconnecting)
            {
                chr = DbClient.GetChar(user.Account.AccountId, CharId);
                if (chr == null)
                {
                    user.SendFailure(Failure.DEFAULT, "Failed to load character.");
                    return;
                }
            }

            if (chr == null)
            {
                user.SendFailure(Failure.DEFAULT, "Invalid reconnect state.");
                return;
            }

            var world = user.GameInfo.World;
            if (world == null || world.Deleted)
            {
                user.SendFailure(Failure.DEFAULT, "World does not exist.");
                return;
            }
            user.Load(chr, world);
        }

        public override string ToString()
        {
            var type = typeof(Load);
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
