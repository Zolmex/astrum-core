using Common;
using Common.Database;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Specialized;

namespace AppEngine.Handlers.Account
{
    public class Verify : RequestHandler
    {
        public override string Path => "/account/verify";

        public override string Handle(string ip, NameValueCollection query)
        {
            var acc = DbClient.VerifyAccount(query["username"], query["password"], out var status);
            if (acc == null)
                return status.GetDescription();
            return acc.ToXml().ToString();
        }
    }
}
