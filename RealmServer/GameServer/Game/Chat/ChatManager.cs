using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Chat
{
    public static class ChatManager
    {
        private static readonly Logger _log = new Logger(typeof(ChatManager));

        public static void Announce(string text)
        {
            var msg = $"<ANNOUNCEMENT> {text}";
            foreach (var kvp in RealmManager.Users)
            {
                var user = kvp.Value;
                if (user.State == ConnectionState.Ready && user.GameInfo.State == GameState.Playing)
                {
                    var plr = user.GameInfo.Player;
                    plr.SendInfo(msg);
                }
            }
            _log.Debug(msg);
        }
    }
}
