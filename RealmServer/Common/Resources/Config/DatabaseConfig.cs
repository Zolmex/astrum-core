using Common.Utilities;
using System.IO;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public class DatabaseConfig
    {
        private const string ConfigFile = "Resources/Config/Data/databaseConfig.xml";

        private static DatabaseConfig _config;
        public static DatabaseConfig Config
            => _config ??= Load();

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Password { get; private set; }
        public int DbIndex { get; private set; }
        public int MaxAccountsPerIP { get; private set; }

        public DatabaseConfig(XElement e)
        {
            Host = e.GetValue<string>("Host");
            Port = e.GetValue<int>("Port");
            Password = e.GetValue<string>("Password");
            DbIndex = e.GetValue<int>("DbIndex");
            MaxAccountsPerIP = e.GetValue<int>("MaxAccountsPerIP");
        }

        private static DatabaseConfig Load()
            => new DatabaseConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}