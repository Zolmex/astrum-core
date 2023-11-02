using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.PlayerCommands
{
    public class OnlineCommand : PlayerCommand
    {
        public override string Name => "/online";
        public override bool AdminOnly => false;

        public override void Execute(Player player, string args)
        {
            var totalCount = RealmManager.Users.Count;
            var localCount = player.World.Players.Count;
            player.SendInfo($"There are {totalCount} players online. {localCount} of them are in this world.");
        }
    }
}
