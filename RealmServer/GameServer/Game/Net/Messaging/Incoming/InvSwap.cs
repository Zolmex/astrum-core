using Common;
using Common.Resources.Xml;
using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class InvSwap : IncomingPacket
    {
        public SlotObjectData SlotObject1 { get; set; }
        public SlotObjectData SlotObject2 { get; set; }

        public override PacketId ID => PacketId.INVSWAP;

        protected override void Read(NetworkReader rdr)
        {
            SlotObject1 = SlotObjectData.Read(rdr);
            SlotObject2 = SlotObjectData.Read(rdr);
        }

        protected override void Handle(User user)
        {
            // TODO
        }

        public override string ToString()
        {
            var type = typeof(InvSwap);
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
