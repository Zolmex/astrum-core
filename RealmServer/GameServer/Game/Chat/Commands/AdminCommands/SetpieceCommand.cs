using Common.Resources.World;
using Common.Resources.Xml;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;
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
    public class SetpieceCommand : PlayerCommand
    {
        public override string Name => "/setpiece";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            var words = args.Split(' ');
            if (words.Length < 1)
            {
                player.SendHelp("Usage: /setpiece (setpiece name) (map index, default 0)");
                return;
            }

            var mapIndex = 0;
            if (words.Length > 1)
            {
                mapIndex = int.Parse(words[1]);
            }

            if (!WorldLibrary.MapDatas.TryGetValue(words[0], out var setpiece))
            {
                player.SendError($"Invalid setpiece: {words[0]}");
                return;
            }

            var map = setpiece[mapIndex];
            player.World.SpawnSetPiece(words[0], (int)player.Position.X - map.Width / 2, (int)player.Position.Y - map.Height / 2, mapIndex);
        }
    }
}
