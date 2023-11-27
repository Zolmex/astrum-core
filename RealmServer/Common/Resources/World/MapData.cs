using Common.Resources.Xml;
using Common.Utilities;
using Common.Utilities.Net;
using Ionic.Zlib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Unicode;
using static System.Net.WebRequestMethods;

namespace Common.Resources.World
{
    public enum TileRegion
    {
        None,
        Spawn,
        Regen,
        Blocks_Sight,
        Note,
        Enemy_1,
        Enemy_2,
        Enemy_3,
        Enemy_4,
        Enemy_5,
        Enemy_6,
        Decoration_1,
        Decoration_2,
        Decoration_3,
        Decoration_4,
        Decoration_5,
        Decoration_6,
        Trigger_1,
        Callback_1,
        Trigger_2,
        Callback_2,
        Trigger_3,
        Callback_3,
        Trigger_4,
        Callback_4,
        Store_1,
        Store_2,
        Store_3,
        Store_4,
        Realm_Portals,
        Vault,
        Gifting_Chest
    }

    public struct json_dat
    {
        public byte[] data { get; set; }
        public loc[] dict { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public struct loc
    {
        public string ground { get; set; }
        public obj[] objs { get; set; }
        public obj[] regions { get; set; }
    }

    public struct obj
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class MapTileData
    {
        public ushort GroundType;
        public ushort ObjectType;
        public TileRegion Region;
        public string Key;

        // Wmap / Realm
        public string ObjectCfg;
        public byte Elevation;
        public TerrainType Terrain;
    }

    // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/JSMap.cs#L55
    public class MapData
    {
        private readonly Logger _log = new Logger(typeof(MapData));

        public MapTileData[,] Tiles;
        public int Width;
        public int Height;
        public Dictionary<TileRegion, List<IntPoint>> Regions;

        public MapData(byte[] data, string mapName)
        {
            bool wmap = mapName.EndsWith(".wmap");
            if (wmap)
            {
                LoadWMap(data);
                InitRegions();
                return;
            }

            string str = Encoding.UTF8.GetString(data);
            json_dat json = JsonConvert.DeserializeObject<json_dat>(str);
            byte[] buffer = ZlibStream.UncompressBuffer(json.data);
            Dictionary<ushort, MapTileData> dict = new Dictionary<ushort, MapTileData>();
            MapTileData[,] tiles = new MapTileData[json.width, json.height];

            for (int i = 0; i < json.dict.Length; i++)
            {
                loc o = json.dict[i];

                TileRegion region = TileRegion.None;
                if (o.regions != null)
                {
                    var regionSuccess = Enum.TryParse(o.regions[0].id.Replace(' ', '_'), out region);
                    if (!regionSuccess)
                        _log.Warn($"Map region unknown. Map name: {mapName} Region: {o.regions[0].id}");
                }

                dict[(ushort)i] = new MapTileData
                {
                    GroundType = o.ground == null ? (ushort)255 : XmlLibrary.Id2Tile(o.ground)?.GroundType ?? 0,
                    ObjectType = o.objs == null ? (ushort)255 : XmlLibrary.Id2Object(o.objs[0].id)?.ObjectType ?? 0,
                    Key = o.objs?[0].name,
                    Region = region
                };
            }

            using (NetworkReader rdr = new NetworkReader(new MemoryStream(buffer)))
                for (int y = 0; y < json.height; y++)
                    for (int x = 0; x < json.width; x++)
                        tiles[x, y] = dict[(ushort)rdr.ReadInt16()];

            //Add composite under cave walls
            for (int x = 0; x < json.width; x++)
                for (int y = 0; y < json.height; y++)
                    if (tiles[x, y].ObjectType != 255)
                    {
                        if (XmlLibrary.ObjectDescs.TryGetValue(tiles[x, y].ObjectType, out var desc))
                            if ((desc.CaveWall || desc.ConnectedWall) && tiles[x, y].GroundType == 255)
                                tiles[x, y].GroundType = 0xfd;
                    }

            Tiles = tiles;
            Width = json.width;
            Height = json.height;

            InitRegions();
        }

        public void InitRegions()
        {
            Regions = new Dictionary<TileRegion, List<IntPoint>>();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    MapTileData tile = Tiles[x, y];
                    if (tile.Region == TileRegion.None)
                        continue;

                    if (!Regions.ContainsKey(tile.Region))
                        Regions[tile.Region] = new List<IntPoint>();
                    Regions[tile.Region].Add(new IntPoint(x, y));
                }
        }

        private void LoadWMap(byte[] data)
        {
            var stream = new MemoryStream(data);
            var ver = stream.ReadByte();

            if (ver is < 0 or > 2)
                throw new NotSupportedException("WMap version " + ver);

            using var rdr = new BinaryReader(new ZlibStream(stream, CompressionMode.Decompress));

            var tileDatas = new MapTileData[rdr.ReadInt16()];
            for (var i = 0; i < tileDatas.Length; i++)
            {
                var tileType = rdr.ReadUInt16();
                var objId = rdr.ReadString();
                var tile = new MapTileData()
                {
                    GroundType = tileType,
                    ObjectType = !string.IsNullOrEmpty(objId) ? XmlLibrary.Id2Object(objId).ObjectType : (ushort)0,
                    ObjectCfg = rdr.ReadString(),
                    Terrain = (TerrainType)rdr.ReadByte(),
                    Region = (TileRegion)rdr.ReadByte(),
                    Elevation = ver == 1 ? rdr.ReadByte() : (byte)0
                };

                tileDatas[i] = tile;
            }

            Width = rdr.ReadInt32();
            Height = rdr.ReadInt32();
            Tiles = new MapTileData[Width, Height];

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    var tile = tileDatas[rdr.ReadInt16()];

                    if (ver == 2)
                        tile.Elevation = rdr.ReadByte();

                    Tiles[x, y] = tile;
                }
        }
    }
}
