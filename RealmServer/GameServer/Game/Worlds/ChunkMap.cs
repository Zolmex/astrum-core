using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Worlds
{
    public class ChunkMap
    {
        public const float CHUNK_SIZE = 32;

        public MapChunk this[int x, int y] => _chunks[x, y];

        private readonly MapChunk[,] _chunks;
        public readonly int Width;
        public readonly int Height;

        public ChunkMap(int width, int height)
        {
            Width = (int)Math.Ceiling(width / CHUNK_SIZE) + 1; // Include the full width and height of the map
            Height = (int)Math.Ceiling(height / CHUNK_SIZE) + 1;

            _chunks = new MapChunk[width, height];
            for (int cY = 0; cY < Height; cY++)
                for (int cX = 0; cX < Width; cX++)
                {
                    var chunk = new MapChunk(cX, cY);
                    _chunks[cX, cY] = chunk;
                }
        }
    }
}
