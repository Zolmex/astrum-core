using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Worlds
{
    public class MapChunk
    {
        public int CX { get; private set; }
        public int CY { get; private set; }
        public HashSet<Entity> Entities { get; private set; }

        public int Activity; // Nearby players will increase this value by 1, and decrease it when they get far away

        public MapChunk(int cX, int cY)
        {
            CX = cX;
            CY = cY;
            Entities = new HashSet<Entity>();
        }

        public void Insert(Entity en)
        {
            lock (Entities)
                Entities.Add(en);
        }

        public void Remove(Entity en)
        {
            lock (Entities)
                Entities.Remove(en);
        }

        public void ActivityUp()
        {
            Interlocked.Increment(ref Activity);
        }

        public void ActivityDown()
        {
            if (Activity > 0)
                Interlocked.Decrement(ref Activity);
        }

        public int DistSqr(MapChunk chunk)
        {
            var dx = CX - chunk.CX;
            var dy = CY - chunk.CY;
            return dx * dx + dy * dy;
        }
    }
}
