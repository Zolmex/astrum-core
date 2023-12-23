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
    public class GotoEventCommand : PlayerCommand
    {
        public override string Name => "/gotoEvent";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrEmpty(args))
                args = "self";

            if (player.World is not Realm realm)
            {
                player.SendError("You can only use this command in the realm");
                return;
            }

            var realmEvent = realm.Oryx.ActiveEvent;
            if (realmEvent == null || realmEvent.Dead)
            {
                player.SendError("There is no event alive at the moment.");
                return;
            }

            var reconList = ApplyTo(args, player);
            foreach (var plr in reconList)
                Goto.Write(plr.User.Network,
                    realmEvent.Position
                    );
        }
    }
}
