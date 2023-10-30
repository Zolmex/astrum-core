using Common.Database;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Specialized;

namespace AppEngine.Handlers.Char
{
    public class Delete : RequestHandler
    {
        public override string Path => "/char/delete";

        public override string Handle(string ip, NameValueCollection query)
        {
            var acc = DbClient.VerifyAccount(query["username"], query["password"], out var status);
            if (acc == null)
                return status.GetDescription();

            if (!int.TryParse(query["charId"], out var charId))
                return WriteError("A character Id is required to delete the character");

            var chr = DbClient.GetChar(acc.AccountId, charId);
            if (chr == null)
                return WriteError("Invalid character Id.");

            if (DbClient.DeleteChar(acc, chr))
                return WriteSuccess();

            return WriteError("Internal server error.");
        }
    }
}
