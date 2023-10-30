using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Worlds
{
    public class MapChunk
    {
        public int CX { get; private set; }
        public int CY { get; private set; }

        public int Activity; // Nearby players will increase this value by 1, and decrease it when they get far away

        public MapChunk(int cX, int cY)
        {
            CX = cX;
            CY = cY;
        }

        public void ActivityUp()
        {
            Interlocked.Increment(ref Activity);
        }

        public void ActivityDown()
        {
            Interlocked.Decrement(ref Activity);
        }
    }
}
