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
    public abstract class CommandManager
    {
        private static Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        protected static Logger _log;

        public CommandManager()
        {

        }

        public abstract void Execute(Player player, string args);

        public static IEnumerable<Player> GetPlayers(string args, Player player)
        {
            var playerList = new List<Player>() { player };
            switch (args)
            {
                case "self":
                    return playerList;
                case "nearby":
                    return player.World.GetAllPlayersWithin(player.Position.X, player.Position.Y, Player.SIGHT_RADIUS_SQR);
                case "all":
                    return player.World.GetAllPlayers();
                case "server":
                    return RealmManager.GetActivePlayers();
            }
            return playerList;
        }

        public static void Load()
        {
            _log = new Logger(typeof(CommandManager));

            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(Command)))
                {
                    var cmd = (Command)Activator.CreateInstance(type);
                    var commandAttribute = System.Attribute.GetCustomAttribute(type, typeof(CommandAttribute)) as CommandAttribute;
                    cmd.PermissionLevel = commandAttribute.PermissionLevel;
                    _commands.Add(string.Format("/{0}", commandAttribute.Command), cmd);
                }
            }
        }
        public static void LogError(Exception e)
            => _log.Error(e);

        public static void ExecuteCommand(Player player, string name, string args)
        {
            if (!_commands.TryGetValue(name.ToLower(), out var cmd))
            {
                player.SendError($"Command not found: {name}");
                return;
            }

            if (cmd.PermissionLevel == Command.CommandPermissionLevel.Admin && !player.User.Account.Admin)
            {
                player.SendError("You're not authorized to use this command.");
                return;
            }

            cmd.Execute(player, args);
        }
    }
}
