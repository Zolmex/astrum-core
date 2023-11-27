using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat.Commands
{
    // This is basically the same implementation as RequestHandler in AppEngine
    public abstract class PlayerCommand
    {
        private static Dictionary<string, PlayerCommand> _commands = new Dictionary<string, PlayerCommand>();

        public abstract string Name { get; }
        public abstract bool AdminOnly { get; }

        protected readonly Logger _log;

        public PlayerCommand()
        {
            _log = new Logger(GetType());
        }

        public abstract void Execute(Player player, string args);

        protected List<Player> ApplyTo(string args, Player player)
        {
            var playerList = new List<Player>();
            switch (args)
            {
                case "self":
                    playerList.Add(player);
                    break;
                case "nearby":
                    foreach (var kvp in player.World.Players)
                    {
                        var plr = kvp.Value;
                        if (plr.DistSqr(player) <= Player.SIGHT_RADIUS_SQR)
                            playerList.Add(plr);
                    }
                    break;
                case "all":
                    foreach (var kvp in player.World.Players)
                    {
                        var plr = kvp.Value;
                        playerList.Add(plr);
                    }
                    break;
                case "server":
                    foreach (var kvp in RealmManager.Users)
                    {
                        var user = kvp.Value;
                        if (user.State == ConnectionState.Ready && user.GameInfo.State == GameState.Playing)
                            playerList.Add(user.GameInfo.Player);
                    }
                    break;
            }
            return playerList;
        }

        public static void Load()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(PlayerCommand)))
                {
                    var cmd = (PlayerCommand)Activator.CreateInstance(type);
                    _commands.Add(cmd.Name.ToLower(), cmd);
                }
            }
        }

        public static void ExecuteCommand(Player player, string name, string args)
        {
            if (!_commands.TryGetValue(name.ToLower(), out var cmd))
            {
                player.SendError($"Command not found: {name}");
                return;
            }

            if (cmd.AdminOnly && !player.User.Account.Admin)
            {
                player.SendError("You're not authorized to use this command.");
                return;
            }

            cmd.Execute(player, args);
        }
    }
}
