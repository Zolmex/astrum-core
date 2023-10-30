using Common.Resources.Xml;
using Common.Utilities;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class InvDrop : IncomingPacket
    {
        public byte SlotId { get; set; }

        public override PacketId ID => PacketId.INVDROP;

        protected override void Read(NetworkReader rdr)
        {
            SlotId = rdr.ReadByte();
        }

        protected override void Handle(User user)
        {
            // TODO
        }

        public override string ToString()
        {
            var type = typeof(InvDrop);
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
