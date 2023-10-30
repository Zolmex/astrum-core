using Common.Utilities;
using System.IO;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public class GameServerConfig
    {
        private const string ConfigFile = "Resources/Config/Data/gameServerConfig.xml";

        private static GameServerConfig _config;
        public static GameServerConfig Config
            => _config ??= Load();

        public string XmlsDir { get; private set; }
        public string WorldsDir { get; private set; }
        public int Port { get; private set; }
        public string Address { get; private set; }
        public string ServerName { get; private set; }
        public int TPS { get; private set; }
        public int MsPT { get; private set; }
        public int MaxPlayers { get; private set; }
        public int MaxClientsPerIP { get; private set; }
        public string Version { get; private set; }
        public bool AdminOnly { get; private set; }

        public GameServerConfig(XElement e)
        {
            XmlsDir = e.GetValue<string>("XmlsDir");
            WorldsDir = e.GetValue<string>("WorldsDir");
            Port = e.GetValue<int>("Port");
            Address = e.GetValue<string>("Address");
            ServerName = e.GetValue<string>("ServerName");
            TPS = e.GetValue<int>("TPS");
            MsPT = 1000 / TPS;
            MaxPlayers = e.GetValue<int>("MaxPlayers");
            MaxClientsPerIP = e.GetValue<int>("MaxClientsPerIP");
            Version = e.GetValue<string>("Version");
            AdminOnly = e.HasElement("AdminOnly");
        }

        private static GameServerConfig Load()
            => new GameServerConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}