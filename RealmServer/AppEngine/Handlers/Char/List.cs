using Common.Database;
using StackExchange.Redis;
using System;
using System.Collections.Specialized;

namespace AppEngine.Handlers.Char
{
    public class List : RequestHandler
    {
        public override string Path => "/char/list";

        public override string Handle(string ip, NameValueCollection query)
        {
            var acc = DbClient.VerifyAccount(query["username"], query["password"], out _) ?? DbClient.GetGuestAccount();
            return acc.ToCharListXml().ToString();
        }
    }
}
