using Common.Utilities.Net;
using System.IO;

namespace GameServer.Game.Network.Messaging
{
    public abstract class IncomingPacket : Packet
    {
        protected abstract void Handle(User user);
        public void ReadBody(User user, NetworkReader rdr)
        {
            // Read packet body
            Read(rdr);
            Handle(user);
        }
        protected abstract void Read(NetworkReader rdr);
    }
}
