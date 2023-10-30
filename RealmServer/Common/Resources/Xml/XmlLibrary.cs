using Common.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml
{
    public static class XmlLibrary
    {
        private static readonly Logger Log = new Logger(typeof(XmlLibrary));

        public static readonly Dictionary<ushort, ObjectDesc> ObjectDescs = new Dictionary<ushort, ObjectDesc>();
        public static readonly Dictionary<ushort, ContainerDesc> ContainerDescs = new Dictionary<ushort, ContainerDesc>();
        public static readonly Dictionary<ushort, PlayerDesc> PlayerDescs = new Dictionary<ushort, PlayerDesc>();
        public static readonly Dictionary<ushort, Item> ItemDescs = new Dictionary<ushort, Item>();
        public static readonly Dictionary<ushort, TileDesc> TileDescs = new Dictionary<ushort, TileDesc>();

        /// <summary>
        /// Loads every .xml file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing XML asset files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*xml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Log.Debug($"Loading XML {file}...");
                MakeDictionaries(XElement.Parse(File.ReadAllText(file)));
            }
            Log.Info("XML Library loaded successfully.");
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
                    ObjectDescs.Add(type, new ObjectDesc(xml, id, type));
                }
                if (name == "Ground")
                    TileDescs.Add(type, new TileDesc(xml, id, type));
            }
        }

        public static ObjectDesc Id2Object(string id)
        {
            var ret = ObjectDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                throw new KeyNotFoundException($"ObjectId: {id}");
            return ret;
        }

        public static PlayerDesc Id2Player(string id)
        {
            var ret = PlayerDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                throw new KeyNotFoundException($"ObjectId: {id}");
            return ret;
        }

        public static Item Id2Item(string id)
        {
            var ret = ItemDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                throw new KeyNotFoundException($"ObjectId: {id}");
            return ret;
        }

        public static TileDesc Id2Tile(string id)
        {
            var ret = TileDescs.Values.FirstOrDefault(desc => desc.ObjectId.EqualsIgnoreCase(id));
            if (ret == null)
                throw new KeyNotFoundException($"ObjectId: {id}");
            return ret;
        }
    }
}
