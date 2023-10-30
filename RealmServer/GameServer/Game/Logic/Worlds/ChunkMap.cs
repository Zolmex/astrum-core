using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Worlds
{
    public class ChunkMap
    {
        public const float CHUNK_SIZE = 32;

        public MapChunk this[int x, int y] => _chunks[x, y];

        private readonly MapChunk[,] _chunks;
        private readonly int _cWidth;
        private readonly int _cHeight;

        public ChunkMap(int width, int height)
        {
            _cWidth = (int)Math.Ceiling(width / CHUNK_SIZE);
            _cHeight = (int)Math.Ceiling(height / CHUNK_SIZE);

            _chunks = new MapChunk[width, height];
            for (int cY = 0; cY <= _cHeight; cY++) // Include the full width and height of the map
                for (int cX = 0; cX <= _cWidth; cX++)
                {
                    var chunk = new MapChunk(cX, cY);
                    _chunks[cX, cY] = chunk;
                }
        }
    }
}
