using GameServer.Game.Chat.Commands;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.AdminCommands
{
    public class AnnounceCommand : PlayerCommand
    {
        public override string Name => "/announce";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            ChatManager.Announce(args);
        }
    }
}
