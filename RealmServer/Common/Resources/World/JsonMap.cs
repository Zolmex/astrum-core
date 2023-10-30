using Common.Resources.Xml;
using Common.Utilities.Net;
using Ionic.Zlib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
        Store_4
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

    public class JsonTile
    {
        public ushort GroundType;
        public ushort ObjectType;
        public TileRegion Region;
        public string Key;
    }

    // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/JSMap.cs#L55
    public class JsonMap
    {
        public JsonTile[,] Tiles;
        public int Width;
        public int Height;
        public Dictionary<TileRegion, List<IntPoint>> Regions;

        public JsonMap(string data)
        {
            json_dat json = JsonConvert.DeserializeObject<json_dat>(data);
            byte[] buffer = ZlibStream.UncompressBuffer(json.data);
            Dictionary<ushort, JsonTile> dict = new Dictionary<ushort, JsonTile>();
            JsonTile[,] tiles = new JsonTile[json.width, json.height];

            for (int i = 0; i < json.dict.Length; i++)
            {
                loc o = json.dict[i];
                dict[(ushort)i] = new JsonTile
                {
                    GroundType = o.ground == null ? (ushort)255 : XmlLibrary.Id2Tile(o.ground).ObjectType,
                    ObjectType = o.objs == null ? (ushort)255 : XmlLibrary.Id2Object(o.objs[0].id).ObjectType,
                    Key = o.objs?[0].name,
                    Region = o.regions == null ? TileRegion.None : (TileRegion)Enum.Parse(typeof(TileRegion), o.regions[0].id.Replace(' ', '_'))
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
                        ObjectDesc desc = XmlLibrary.ObjectDescs[tiles[x, y].ObjectType];
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
                    JsonTile tile = Tiles[x, y];
                    if (!Regions.ContainsKey(tile.Region))
                        Regions[tile.Region] = new List<IntPoint>();
                    Regions[tile.Region].Add(new IntPoint(x, y));
                }
        }
    }
}
