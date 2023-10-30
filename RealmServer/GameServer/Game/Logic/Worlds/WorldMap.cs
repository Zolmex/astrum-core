using Common;
using Common.Resources.World;
using Common.Utilities.Net;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Game.Logic.Worlds
{
    public class WorldTile
    {
        public int X { get; }
        public int Y { get; }
        public ushort GroundType { get; }
        public ushort ObjectType { get; }
        public TileRegion Region { get; }
        public Entity Object { get; set; }
        public bool BlocksSight { get; set; }
        public MapChunk Chunk { get; set; }

        public WorldTile(JsonTile tile, int x, int y, MapChunk chunk)
        {
            X = x;
            Y = y;
            Chunk = chunk;
            GroundType = tile.GroundType;
            ObjectType = tile.ObjectType;
            Region = tile.Region;
        }

        public void Write(NetworkWriter wtr)
        {
            wtr.Write((short)X);
            wtr.Write((short)Y);
            wtr.Write(GroundType);
        }
    }

    public class WorldMap
    {
        public int Width { get; }
        public int Height { get; }
        public Dictionary<WorldPosData, TileRegion> Regions { get; }
        public Entity[] Entities { get; }
        public WorldTile[,] Tiles { get; }
        public ChunkMap Chunks { get; set; }
        public WorldTile this[int x, int y]
        {
            get => Tiles[x, y];
            set => Tiles[x, y] = value;
        }

        // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/World.cs#L80
        public WorldMap(JsonMap map)
        {
            Width = map.Width;
            Height = map.Height;
            Entities = new Entity[0];
            Tiles = new WorldTile[Width, Height];
            Chunks = new ChunkMap(Width, Height);
            Regions = new Dictionary<WorldPosData, TileRegion>();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    var cX = (int)Math.Ceiling(x / ChunkMap.CHUNK_SIZE);
                    var cY = (int)Math.Ceiling(y / ChunkMap.CHUNK_SIZE);
                    var chunk = Chunks[cX, cY];
                    var js = map.Tiles[x, y];
                    var tile = Tiles[x, y] = new WorldTile(js, x, y, chunk);
                    if (js.ObjectType != 0xff)
                    {
                        var entity = Entity.Resolve(js.ObjectType);
                        if (entity.Desc.Static)
                        {
                            if (entity.Desc.BlocksSight)
                                tile.BlocksSight = true;
                            tile.Object = entity;
                        }

                        entity.Move(x + 0.5f, y + 0.5f);
                        Entities = Entities.Append(entity).ToArray();
                    }

                    if (tile.Region != TileRegion.None)
                    {
                        var pos = new WorldPosData() { X = x, Y = y };
                        Regions[pos] = tile.Region;
                    }
                }
        }
    }
}
