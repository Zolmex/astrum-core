using Common;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities.Net;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Game.Worlds
{
    public class WorldTile
    {
        public readonly TileDesc TileDesc;
        public readonly MapTileData Data;
        public int X { get; }
        public int Y { get; }
        public ushort GroundType { get; private set; }
        public ushort ObjectType { get; private set; }
        public TileRegion Region { get; private set; }
        public Entity Object { get; set; }
        public bool BlocksSight { get; set; }
        public MapChunk Chunk { get; set; }
        public Portal Portal { get; set; }

        public WorldTile(MapTileData tile, int x, int y, MapChunk chunk)
        {
            TileDesc = XmlLibrary.TileDescs[tile.GroundType];

            Data = tile;
            X = x;
            Y = y;
            Chunk = chunk;
            GroundType = tile.GroundType;
            ObjectType = tile.ObjectType;
            Region = tile.Region;
        }

        public void Update(MapTileData newTile)
        {
            GroundType = newTile.GroundType;
            ObjectType = newTile.ObjectType;
            Region = newTile.Region;
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
        public List<Entity> Entities { get; }
        public WorldTile[,] Tiles { get; }
        public ChunkMap Chunks { get; set; }
        public WorldTile this[int x, int y]
        {
            get => Tiles[x, y];
            set => Tiles[x, y] = value;
        }

        // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/World.cs#L80
        public WorldMap(MapData map)
        {
            // TODO: cache this

            Width = map.Width;
            Height = map.Height;
            Entities = new List<Entity>();
            Tiles = new WorldTile[Width, Height];
            Chunks = new ChunkMap(Width, Height);
            Regions = new Dictionary<WorldPosData, TileRegion>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var cX = (int)(x / ChunkMap.CHUNK_SIZE);
                    var cY = (int)(y / ChunkMap.CHUNK_SIZE);
                    var chunk = Chunks[cX, cY];
                    var js = map.Tiles[x, y];
                    var tile = Tiles[x, y] = new WorldTile(js, x, y, chunk);
                    if (js.ObjectType != 0xff && js.ObjectType != 0)
                    {
                        var entity = Entity.Resolve(js.ObjectType);
                        if (entity.Desc.Static)
                        {
                            if (entity.Desc.BlocksSight)
                                tile.BlocksSight = true;
                            tile.Object = entity;
                        }

                        entity.Move(x + 0.5f, y + 0.5f);
                        Entities.Add(entity);
                    }

                    if (tile.Region != TileRegion.None)
                    {
                        var pos = new WorldPosData() { X = x, Y = y };
                        Regions[pos] = tile.Region;
                    }
                }
        }

        public bool Contains(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= Width || y >= Height);
        }
    }
}
