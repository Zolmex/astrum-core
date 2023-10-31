using Common.Utilities;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

        public static void Load()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(PlayerCommand)))
                {
                    var cmd = (PlayerCommand)Activator.CreateInstance(type);
                    _commands.Add(cmd.Name, cmd);
                }
            }
        }

        public static void ExecuteCommand(Player player, string name, string args)
        {
            if (!_commands.TryGetValue(name, out var cmd))
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
