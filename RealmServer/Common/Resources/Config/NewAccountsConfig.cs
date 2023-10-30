using Common.Utilities;
using System.IO;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public class NewAccountsConfig
    {
        private const string ConfigFile = "Resources/Config/Data/newAccountsConfig.xml";

        private static NewAccountsConfig _config;
        public static NewAccountsConfig Config
            => _config ??= Load();

        public int Fame { get; private set; }
        public int Credits { get; private set; }
        public int MaxChars { get; private set; }
        public int VaultCount { get; private set; }
        public int CharSlotCost { get; private set; }

        public NewAccountsConfig(XElement e)
        {
            Fame = e.GetValue<int>("Fame");
            Credits = e.GetValue<int>("Credits");
            MaxChars = e.GetValue<int>("MaxChars");
            VaultCount = e.GetValue<int>("VaultCount");
            CharSlotCost = e.GetValue<int>("CharSlotCost");
        }

        private static NewAccountsConfig Load()
            => new NewAccountsConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}