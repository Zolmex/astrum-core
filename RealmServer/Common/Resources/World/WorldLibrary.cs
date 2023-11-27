using Common.Resources.Config;
using Common.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Common.Resources.World
{
    public static class WorldLibrary
    {
        private static readonly Logger Log = new Logger(typeof(WorldLibrary));

        public static readonly Dictionary<string, WorldConfig> WorldConfigs = new Dictionary<string, WorldConfig>();
        public static readonly Dictionary<string, MapData[]> MapDatas = new Dictionary<string, MapData[]>();

        /// <summary>
        /// Loads every .json file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing world config and map files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*json", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Log.Debug($"Loading world {file}...");

                var config = JsonConvert.DeserializeObject<WorldConfig>(File.ReadAllText(file));
                var maps = DeserializeMaps(config);
                WorldConfigs[config.Name] = config;
                MapDatas[config.Name] = maps;
            }
            Log.Info("World Library loaded successfully.");
        }

        private static MapData[] DeserializeMaps(WorldConfig config)
        {
            var list = new List<MapData>();
            foreach (string mapName in config.Maps)
            {
                var data = File.ReadAllBytes(GameServerConfig.Config.WorldsDir + mapName);
                list.Add(new MapData(data, mapName));
            }
            return list.ToArray();
        }
    }
}
