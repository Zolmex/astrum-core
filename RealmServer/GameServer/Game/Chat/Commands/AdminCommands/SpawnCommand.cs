using Common.Database;
using Common.Resources.Xml;
using Common;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.AdminCommands
{
    public class SpawnCommand : PlayerCommand
    {
        public override string Name => "/spawn";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                player.SendHelp("/spawn <count> <entity>");
                return;
            }
            string[] rgs = args.Split(' ');

            int spawnCount;
            if (!int.TryParse(rgs[0], out spawnCount))
                spawnCount = -1;

            ObjectDesc desc = XmlLibrary.Id2Object(string.Join(' ', spawnCount == -1 ? rgs : rgs.Skip(1)));
            if (spawnCount == -1) spawnCount = 1;
            if (desc == null)
            {
                player.SendError("null object desc");
                return;
            }

            if (desc.Player || desc.Static)
            {
                player.SendError("Can't spawn this entity");
                return;
            }

            player.SendInfo($"Spawning <{spawnCount}> <{desc.DisplayId}> in 2 seconds");

            WorldPosData pos = player.Position;

            RealmManager.AddTimedAction(2000, () =>
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Entity entity = Entity.Resolve(desc.ObjectType);
                    entity.Position = pos;
                    entity.EnterWorld(player.World);
                }
            });
        }
    }
}
