using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Database
{
    internal static class DbKeys
    {
        public const string OnlineAccounts = "online"; // Player names go here, used for acc in use and to get player count
        public const string AccountNames = "accNames";
        public const string AccountIPs = "accIPs";
        public const string AccountLogins = "logins";
        public const string NextAccountId = "nextAccId";
    }
}
