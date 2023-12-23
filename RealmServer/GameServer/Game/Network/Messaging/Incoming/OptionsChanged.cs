using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Game.Entities;

namespace GameServer.Game.Network.Messaging.Incoming
{
    public class OptionsChanged : IncomingPacket
    {
        public byte AllyShots { get; set; }
        public byte AllyDamage { get; set; }
        public byte AllyNotifs { get; set; }
        public byte AllyParticles { get; set; }
        public byte AllyEntities { get; set; }

        public override PacketId ID => PacketId.OPTIONS_CHANGED;

        protected override void Read(NetworkReader rdr)
        {
            AllyShots = rdr.ReadByte();
            AllyDamage = rdr.ReadByte();
            AllyNotifs = rdr.ReadByte();
            AllyParticles = rdr.ReadByte();
            AllyEntities = rdr.ReadByte();
        }

        protected override void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            user.GameInfo.AllySettings(AllyShots, AllyDamage, AllyNotifs, AllyParticles, AllyEntities);
        }

        public override string ToString()
        {
            var type = typeof(UsePortal);
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
