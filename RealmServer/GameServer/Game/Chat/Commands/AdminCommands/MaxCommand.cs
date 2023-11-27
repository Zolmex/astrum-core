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
    public class MaxCommand : PlayerCommand
    {
        public override string Name => "/max";
        public override bool AdminOnly => true;

        public override void Execute(Player player, string args)
        {
            Player target = null;
            if (args == null)
                target = player;
            else
                foreach (var kvp in player.World.Players)
                {
                    var plr = kvp.Value;

                    if (plr.Name.ToLower() == args.ToLower())
                    {
                        target = plr;
                        break;
                    }
                }

            if (target == null)
            {
                player.SendError($"Player '{args}' not found.");
                return;
            }

            var classDesc = XmlLibrary.PlayerDescs[target.Desc.ObjectType];
            target.HP = classDesc.Stats[0].MaxValue;
            target.MP = classDesc.Stats[1].MaxValue;
            target.MaxHP = classDesc.Stats[0].MaxValue;
            target.MaxMP = classDesc.Stats[1].MaxValue;
            target.Attack = classDesc.Stats[2].MaxValue;
            target.Defense = classDesc.Stats[3].MaxValue;
            target.Speed = classDesc.Stats[4].MaxValue;
            target.Dexterity = classDesc.Stats[5].MaxValue;
            target.Vitality = classDesc.Stats[6].MaxValue;
            target.Wisdom = classDesc.Stats[7].MaxValue;
            target.SaveCharacter();
        }
    }
}
