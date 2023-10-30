using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.PlayerCommands
{
    public class UptimeCommand : PlayerCommand
    {
        public override string Name => "/uptime";
        public override bool AdminOnly => false;

        public override void Execute(Player player, string args)
        {
            var time = TimeSpan.FromMilliseconds(RealmManager.GlobalTime.TotalElapsedMs);
            player.SendInfo($"The server has been up for {time.Days} days, {time.Hours} hours and {time.Seconds} seconds.");
        }
    }
}
