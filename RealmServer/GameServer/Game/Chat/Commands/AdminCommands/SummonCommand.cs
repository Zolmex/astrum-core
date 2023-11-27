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
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.AdminCommands
{
    public class SummonCommand : PlayerCommand
    {
        public override string Name => "/summon";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                player.SendHelp("Usage: /summon (player name or all)");
                return;
            }

            List<Player> targets = new List<Player>();
            foreach (var kvp in player.World.Players)
            {
                var plr = kvp.Value;
                if (args == "all" || plr.Name.ToLower() == args.ToLower())
                {
                    targets.Add(plr);

                    if (args != "all")
                        break;
                }
            }

            if (targets.Count == 0)
            {
                player.SendError("There is no event alive at the moment.");
                return;
            }

            foreach (var target in targets)
                target.User.SendPacket(PacketId.GOTO, Goto.Write(target.User,
                    player.Position
                    ));
        }
    }
}
