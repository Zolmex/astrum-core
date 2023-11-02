using Common;
using Common.Database;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private const int TextCooldown = 500;

        private long _lastMessageSent;

        public void SendInfo(string text)
        {
            User.SendPacket(PacketId.TEXT, Text.Write(User,
                "",
                0,
                -1,
                (byte)0,
                null,
                text));
        }

        public void SendError(string text)
        {
            User.SendPacket(PacketId.TEXT, Text.Write(User,
                "*Error*",
                0,
                -1,
                (byte)0,
                null,
                text));
        }

        public void SendHelp(string text)
        {
            User.SendPacket(PacketId.TEXT, Text.Write(User,
                "*Help*",
                0,
                -1,
                (byte)0,
                null,
                text));
        }

        public void SendEnemy(Entity entity, string text)
        {
            User.SendPacket(PacketId.TEXT, Text.Write(User,
                $"#{entity.Desc.DisplayName}",
                entity.Id,
                -1,
                (byte)3,
                null,
                text));
        }

        public void Speak(string text)
        {
            if (!ValidateSpeak(RealmManager.GlobalTime, text))
                return;

            if (text.StartsWith('/'))
            {
                ExecuteCommand(text);
                return;
            }

            var acc = User.Account;

            // Speak to nearby entities
            //foreach (var en in World.ChunkManager.HitTest(X, Y, SightRadius))
            //    en.ChatReceived(Id, text);

            // Send text message to players in current world
            foreach (var kvp in World.Players)
            {
                var user = kvp.Value.User;
                if (!user.Account.IgnoredIds?.Contains(acc.AccountId) ?? true)
                    user.SendPacket(PacketId.TEXT, Text.Write(user,
                        acc.Admin ? $"@{acc.Name}" : acc.Name,
                        Id,
                        NumStars,
                        (byte)5,
                        null,
                        text));
            }
        }

        private bool ValidateSpeak(RealmTime time, string text)
        {
            if (User.Account.Admin)
                return true;

            // If desired, word filter goes here

            if (time.TotalElapsedMs - _lastMessageSent < TextCooldown)
                return false;

            _lastMessageSent = time.TotalElapsedMs;
            return true;
        }

        public void ExecuteCommand(string text)
        {
            var spaceIndex = text.IndexOf(' ');
            var command = text.Substring(0, spaceIndex == -1 ? text.Length : spaceIndex);
            var args = spaceIndex == -1 ? null : text.Substring(spaceIndex + 1);
            PlayerCommand.ExecuteCommand(this, command, args);
        }
    }
}
