using Common.Utilities;
using System.IO;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public class AppEngineConfig
    {
        private const string ConfigFile = "Resources/Config/Data/appEngineConfig.xml";

        private static AppEngineConfig _config;
        public static AppEngineConfig Config
            => _config ??= Load();

        public string XmlsDir { get; private set; }
        public string WorldsDir { get; private set; }
        public int Port { get; private set; }
        public string Address { get; private set; }

        public AppEngineConfig(XElement e)
        {
            XmlsDir = e.GetValue<string>("XmlsDir");
            WorldsDir = e.GetValue<string>("WorldsDir");
            Port = e.GetValue<int>("Port");
            Address = e.GetValue<string>("Address");
        }

        private static AppEngineConfig Load()
            => new AppEngineConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}
