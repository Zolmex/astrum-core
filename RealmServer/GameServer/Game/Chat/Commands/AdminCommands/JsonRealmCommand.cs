using Common.Resources.World;
using Common.Resources.Xml;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.AdminCommands
{
    public class JsonRealmCommand : PlayerCommand
    {
        public override string Name => "/jsonRealm";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            var maps = WorldLibrary.MapDatas["Realm"];
            for (var i = 0; i < maps.Length; i++)
            {
                var map = maps[i];
                var json = map.ExportJson();

                if (!Directory.Exists("json/maps"))
                    Directory.CreateDirectory("json/maps");

                File.WriteAllText($"json/maps/Realm{i}.jm", json);
            }
        }
    }
}
