using Common;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameServer.Game.Chat.Commands.Command;

namespace GameServer.Game.Chat.Commands
{
    [Command("announce", CommandPermissionLevel.Admin)]
    public class AnnounceCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            ChatManager.Announce(args);
        }
    }

    [Command("give", CommandPermissionLevel.Admin)]
    public class GiveCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            player.SendError("Command not implemented yet!");
            //i was gonna make a /give command before I noticed the player doesnt have an inventory yet LOL
            //but i didnt want to delete this file because this command will exist eventually
            //HI SHMITTY OR ZOLMEX OR PATPOT OF THE FUTURE WOOOOOOOO
        }
    }

    [Command("gotoEvent", CommandPermissionLevel.Admin)]
    public class GotoEventCommand : Command
    {
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

            var reconList = CommandManager.GetPlayers(args, player);
            foreach (var plr in reconList)
                Goto.Write(plr.User.Network,
                    realmEvent.Position
                    );
        }
    }

    [Command("jsonRealm", CommandPermissionLevel.Admin)]
    public class JsonRealmCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            var maps = WorldLibrary.MapDatas["Realm"];
            for (var i = 0; i < maps.Length; i++)
            {
                var map = maps[i];
                var json = map.ExportJson();

                if (!Directory.Exists("json/maps"))
                    Directory.CreateDirectory("json/maps");

                File.WriteAllText($"json/maps/Realm{i}.jm", json);
            }
        }
    }


    [Command("max", CommandPermissionLevel.Admin)]
    public class MaxCommand : Command
    {
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


    [Command("recon", CommandPermissionLevel.Admin)]
    public class ReconnectCommand : Command
    {
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
                CommandManager.LogError(e);
                player.SendError(e.Message);
                return;
            }

            var reconList = CommandManager.GetPlayers(options[0], player);

            foreach (var plr in reconList)
                plr.User.ReconnectTo(world);
        }
    }

    [Command("setpiece", CommandPermissionLevel.Admin)]
    public class SetpieceCommand : Command
    {
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

    [Command("spawn", CommandPermissionLevel.Admin)]
    public class SpawnCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                player.SendHelp("/spawn <count> <entity>");
                return;
            }
            string[] rgs = args.Split(' ');

            int spawnCount;
            if (!int.TryParse(rgs[0], out spawnCount))
                spawnCount = -1;

            ObjectDesc desc = XmlLibrary.Id2Object(string.Join(' ', spawnCount == -1 ? rgs : rgs.Skip(1)));
            if (spawnCount == -1) spawnCount = 1;
            if (desc == null)
            {
                player.SendError("null object desc");
                return;
            }

            if (desc.Player || desc.Static)
            {
                player.SendError("Can't spawn this entity");
                return;
            }

            player.SendInfo($"Spawning <{spawnCount}> <{desc.DisplayId}> in 2 seconds");

            WorldPosData pos = player.Position;

            RealmManager.AddTimedAction(2000, () =>
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Entity entity = Entity.Resolve(desc.ObjectType);
                    entity.Position = pos;
                    entity.EnterWorld(player.World);
                }
            });
        }
    }

    [Command("summon", CommandPermissionLevel.Admin)]
    public class SummonCommand : Command
    {
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
                return;

            foreach (var target in targets)
                Goto.Write(target.User.Network,
                    player.Position
                    );
        }
    }

    [Command("online", CommandPermissionLevel.All)]
    public class OnlineCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            var totalCount = RealmManager.Users.Count;
            var localCount = player.World.Players.Count;
            player.SendInfo($"There are {totalCount} players online. {localCount} of them are in this world.");
        }
    }

    [Command("realm", CommandPermissionLevel.All)]
    public class RealmCommand : Command
    {
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

    [Command("uptime", CommandPermissionLevel.All)]
    public class UptimeCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            var time = TimeSpan.FromMilliseconds(RealmManager.GlobalTime.TotalElapsedMs);
            player.SendInfo($"The server has been up for {time.Days} days, {time.Hours} hours and {time.Seconds} seconds.");
        }
    }
}
