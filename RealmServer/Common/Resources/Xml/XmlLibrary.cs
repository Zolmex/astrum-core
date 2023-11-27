using Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml
{
    public static class XmlLibrary
    {
        private static readonly Logger _log = new Logger(typeof(XmlLibrary));

        public static readonly Dictionary<ushort, ObjectDesc> ObjectDescs = new Dictionary<ushort, ObjectDesc>();
        public static readonly Dictionary<ushort, ContainerDesc> ContainerDescs = new Dictionary<ushort, ContainerDesc>();
        public static readonly Dictionary<ushort, PlayerDesc> PlayerDescs = new Dictionary<ushort, PlayerDesc>();
        public static readonly Dictionary<ushort, Item> ItemDescs = new Dictionary<ushort, Item>();
        public static readonly Dictionary<ushort, TileDesc> TileDescs = new Dictionary<ushort, TileDesc>();
        public static readonly Dictionary<TerrainType, List<ObjectDesc>> TerrainEnemies = new Dictionary<TerrainType, List<ObjectDesc>>();

        /// <summary>
        /// Loads every .xml file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing XML asset files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*xml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                _log.Debug($"Loading XML {file}...");
                MakeDictionaries(XElement.Parse(File.ReadAllText(file)));
            }
            _log.Info("XML Library loaded successfully.");
        }

        private static void MakeDictionaries(XElement root)
        {
            foreach (var xml in root.Elements())
            {
                var id = xml.GetAttribute<string>("id");
                var type = xml.GetAttribute<ushort>("type");

                var name = xml.Name.ToString();
                if (name == "Object")
                {
                    if (xml.HasElement("Container"))
                        ContainerDescs.Add(type, new ContainerDesc(xml, id, type));
                    else if (xml.HasElement("Player"))
                        PlayerDescs.Add(type, new PlayerDesc(xml, id, type));
                    else if (xml.HasElement("Item"))
                        ItemDescs.Add(type, new Item(xml, id, type));

                    var objDesc = new ObjectDesc(xml, id, type);
                    ObjectDescs.Add(type, objDesc);

                    if (objDesc.Terrain != TerrainType.None/* && xml.HasElement("Spawn")*/) // Entities spawned by Oryx in the Realm
                    {
                        var terrain = Enum.Parse<TerrainType>(xml.Element("Terrain").Value);
                        if (!TerrainEnemies.TryGetValue(terrain, out var list))
                            list = TerrainEnemies[terrain] = new List<ObjectDesc>();
                        list.Add(objDesc);
                    }
                }
                if (name == "Ground")
                    TileDescs.Add(type, new TileDesc(xml, id, type));
            }
        }

        public static ObjectDesc Id2Object(string id)
        {
            var ret = ObjectDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                _log.Warn($"Object id not found: {id}");
            return ret;
        }

        public static PlayerDesc Id2Player(string id)
        {
            var ret = PlayerDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                _log.Warn($"Player id not found: {id}");
            return ret;
        }

        public static Item Id2Item(string id)
        {
            var ret = ItemDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                _log.Warn($"Item id not found: {id}");
            return ret;
        }

        public static TileDesc Id2Tile(string id)
        {
            var ret = TileDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                _log.Warn($"Tile id not found: {id}");
            return ret;
        }
    }
}
