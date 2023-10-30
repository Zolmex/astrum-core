using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Net.Messaging.Outgoing;
using System;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class PlayerShoot : IncomingPacket
    {
        public float Angle { get; set; }
        public bool Ability { get; set; }
        public byte NumShots { get; set; }

        public override PacketId ID => PacketId.PLAYERSHOOT;

        protected override void Read(NetworkReader rdr)
        {
            Angle = rdr.ReadSingle();
            Ability = rdr.ReadBoolean();
            NumShots = rdr.ReadByte();
        }

        protected override void Handle(User user)
        {
            // TODO
        }

        public override string ToString()
        {
            var type = typeof(PlayerShoot);
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
