using Common.Utilities.Net;
using Common.Utilities;
using System;

namespace GameServer.Game.Network.Messaging.Incoming
{
    public class PlayerText : IncomingPacket
    {
        public string Text { get; set; }

        public override PacketId ID => PacketId.PLAYERTEXT;

        protected override void Read(NetworkReader rdr)
        {
            Text = rdr.ReadUTF();
        }

        protected override void Handle(User user)
        {
            user.GameInfo.Player.Speak(Text);
        }

        public override string ToString()
        {
            var type = typeof(PlayerText);
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
