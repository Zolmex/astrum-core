using Common.Utilities;
using GameServer.Game.Logic.Entities;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands.PlayerCommands
{
    public class RealmCommand : PlayerCommand
    {
        public override string Name => "/realm";
        public override bool AdminOnly => false;

        public override void Execute(Player player, string args)
        {
            World realm;
            try
            {
                realm = RealmManager.GetWorld(RealmManager.ActiveRealms.RandomElement());
            }
            catch (Exception)
            {
                player.SendError("No realms available");
                return;
            }
            player.User.ReconnectTo(realm);
        }
    }
}
