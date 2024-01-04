using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameServer.Game.Chat.Commands.Command;

namespace GameServer.Game.Chat.Commands
{
    public class Command
    {
        public CommandPermissionLevel PermissionLevel;
        public virtual void Execute(Player player, string args) { }

        public enum CommandPermissionLevel
        {
            All,
            Admin
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        private string _command;
        public string Command => _command;
        private CommandPermissionLevel _permission;
        public CommandPermissionLevel PermissionLevel => _permission;
        public CommandAttribute(string command, CommandPermissionLevel permissionLevel)
        {
            _command = command;
            _permission = permissionLevel;
        }
    }
}
