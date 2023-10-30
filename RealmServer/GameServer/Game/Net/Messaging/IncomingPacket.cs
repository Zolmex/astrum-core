using Common.Utilities.Net;
using System.IO;

namespace GameServer.Game.Net.Messaging
{
    public abstract class IncomingPacket : Packet
    {
        protected abstract void Handle(User user);
        public void ReadBody(User user, byte[] body, int offset, int length)
        {
            // Read packet body
            Read(new NetworkReader(new MemoryStream(body, offset, length)));
            Handle(user);
        }
        protected abstract void Read(NetworkReader rdr);
    }
}
