using Common.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;

namespace Common.Resources.Config
{
    public class NewsConfig
    {
        private const string ConfigFile = "Resources/Config/Data/newsConfig.xml";

        private static NewsConfig _config;
        public static NewsConfig Config
            => _config ??= Load();

        public NewsItemModel[] Models { get; private set; }

        public NewsConfig(XElement e)
            => Models = JsonConvert.DeserializeObject<NewsItemModel[]>(e.GetValue<string>("Models"));

        private static NewsConfig Load()
            => new NewsConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }

    public class NewsItemModel
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string TagLine { get; set; }
        public string Link { get; set; }
        public int Date { get; set; }

        public XElement ToXml()
            => new XElement("NewsItem",
                new XElement("Icon", Icon),
                new XElement("Title", Title),
                new XElement("TagLine", TagLine),
                new XElement("Link", Link),
                new XElement("Date", Date)
                );
    }
}