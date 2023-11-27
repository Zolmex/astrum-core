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

        public MapChunk this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < Width && y < Height)
                    return _chunks[x, y];
                return null;
            }
        }

        private readonly MapChunk[,] _chunks;
        public readonly int Width;
        public readonly int Height;

        public ChunkMap(int width, int height)
        {
            Width = (int)(width / CHUNK_SIZE); // Include the full width and height of the map
            Height = (int)(height / CHUNK_SIZE);

            _chunks = new MapChunk[Width, Height];
            for (int cY = 0; cY < Height; cY++)
                for (int cX = 0; cX < Width; cX++)
                {
                    var chunk = new MapChunk(cX, cY);
                    _chunks[cX, cY] = chunk;
                }
        }
    }
}
