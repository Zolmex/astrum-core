using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using Common.Resources.Xml;

namespace GameServer.Game.Network.Messaging.Incoming
{
    public class PlayerShoot : IncomingPacket
    {
        public float Angle { get; set; }

        public override PacketId ID => PacketId.PLAYERSHOOT;

        protected override void Read(NetworkReader rdr)
        {
            Angle = rdr.ReadSingle();
        }

        protected override void Handle(User user)
        {
            var player = user.GameInfo.Player;
            if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
                return;

            var weaponType = player.Inventory[0];
            if (weaponType == -1 || weaponType == 0)
                return;

            if (!XmlLibrary.ItemDescs.TryGetValue((ushort)weaponType, out var weapon) || weapon.Projectile == null)
                return;

            player.Shoot(weapon.Projectile, weapon.NumProjectiles, Angle);
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
