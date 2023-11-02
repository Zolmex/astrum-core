using Common.Resources.Xml;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
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
    public class ReconnectCommand : PlayerCommand
    {
        public override string Name => "/recon";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            var options = args.Split(' ');
            if (args.Length < 1 || options.Length < 2)
            {
                player.SendHelp("Usage: /recon (self/nearby/all/server) (world name)");
                return;
            }

            string worldName = options[1];
            if (worldName == "Realm")
            {
                player.SendError("Invalid.");
                return;
            }

            World world;
            try
            {
                world = RealmManager.GetWorld(worldName);
                if (world == null)
                {
                    world = new World(worldName, -1);
                    RealmManager.AddWorld(world);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
                player.SendError(e.Message);
                return;
            }

            var reconList = new List<Player>();
            switch (options[0])
            {
                case "self":
                    reconList.Add(player);
                    break;
                case "nearby":
                    foreach (var kvp in player.World.Players)
                    {
                        var plr = kvp.Value;
                        if (plr.DistSqr(player) <= Player.SIGHT_RADIUS_SQR)
                            reconList.Add(plr);
                    }
                    break;
                case "all":
                    foreach (var kvp in player.World.Players)
                    {
                        var plr = kvp.Value;
                        reconList.Add(plr);
                    }
                    break;
                case "server":
                    foreach (var kvp in RealmManager.Users)
                    {
                        var user = kvp.Value;
                        if (user.State == ConnectionState.Ready && user.GameInfo.State == GameState.Playing)
                            reconList.Add(user.GameInfo.Player);
                    }
                    break;
            }

            foreach (var plr in reconList)
                plr.User.ReconnectTo(world);
        }
    }
}
