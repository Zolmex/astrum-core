using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors
{
    public static class BehaviorLibrary
    {
        private static readonly Logger _log = new Logger(typeof(BehaviorLibrary));

        public static readonly Dictionary<string, BehaviorState> RootStates = new Dictionary<string, BehaviorState>();

        // Manually add new behaviors to this dictionary
        public static void Load(string xmlDir)
        {
            RootStates.Add("Pirate", BehaviorLib.Pirate);

            _log.Debug("Loading XML entity behaviors...");

            LoadXMLs(xmlDir);
        }

        private static void LoadXMLs(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*xml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                _log.Debug($"Loading behavior {file}...");
                ImportXML(XElement.Parse(File.ReadAllText(file)));
            }
            _log.Info("Behavior Library loaded successfully.");
        }

        private static void ImportXML(XElement root)
        {
            foreach (var xml in root.Elements("Behavior"))
            {
                var objId = xml.GetAttribute<string>("id");
                if (RootStates.TryGetValue(objId, out _))
                {
                    _log.Error($"Duplicate behavior for {objId}.");
                    continue;
                }

                RootStates.Add(objId, BehaviorState.LoadFromXML(xml));
            }
        }
    }
}
