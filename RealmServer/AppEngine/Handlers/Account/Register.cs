using Common;
using Common.Database;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Specialized;

namespace AppEngine.Handlers.Account
{
    public class Register : RequestHandler
    {
        public override string Path => "/account/register";

        public override string Handle(string ip, NameValueCollection query)
        {
            var result = DbClient.Register(query["newUsername"], query["newPassword"], ip);
            if (result != RegisterStatus.Success)
                return WriteError(result.GetDescription());
            return WriteSuccess();
        }
    }
}
