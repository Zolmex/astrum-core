using Common.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public enum RealmClose
    {
        EventsKilled,
        MinutesElapsed
    }

    public class RealmCloseData
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public RealmClose CloseType { get; private set; }

        public RealmCloseData(XElement e)
        {
            Min = e.GetAttribute<int>("min");
            Max = e.GetAttribute<int>("max");
            CloseType = Enum.Parse<RealmClose>(e.Value);
        }
    }

    public class RealmEventData
    {
        public string ObjectId { get; private set; }
        public string SetPiece { get; private set; }
        public int PerRealmMax { get; private set; }

        public RealmEventData(XElement e)
        {
            ObjectId = e.GetAttribute<string>("objId");
            SetPiece = e.GetValue<string>("SetPiece");
            PerRealmMax = e.GetValue<int>("PerRealmMax", -1);
        }
    }

    public class RealmConfig
    {
        private const string ConfigFile = "Resources/Config/Data/realmConfig.xml";

        private static RealmConfig _config;
        public static RealmConfig Config
            => _config ??= Load();

        public string[] Names { get; private set; }
        public RealmEventData[] Events { get; private set; }
        public RealmCloseData[] Close { get; private set; }

        public RealmConfig(XElement e)
        {
            Names = e.GetValue<string>("Names")?.Split(',');
            Events = e.Elements("Event").Select(x => new RealmEventData(x)).ToArray();
            Close = e.Elements("Close").Select(x => new RealmCloseData(x)).ToArray();
        }

        private static RealmConfig Load()
            => new RealmConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}