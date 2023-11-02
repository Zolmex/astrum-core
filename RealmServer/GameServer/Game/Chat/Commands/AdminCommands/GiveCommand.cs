using Common.Resources.Xml;
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
    public class GiveCommand : PlayerCommand
    {
        public override string Name => "/give";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            player.SendError("Command not implemented yet!");
            //i was gonna make a /give command before I noticed the player doesnt have an inventory yet LOL
            //but i didnt want to delete this file because this command will exist eventually
            //HI SHMITTY OR ZOLMEX OF THE FUTURE WOOOOOOOO
        }
    }
}
