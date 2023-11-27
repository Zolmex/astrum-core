using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common.Database
{
    public static class DbClient
    {
        private static readonly Logger _log = new Logger(typeof(DbClient));

        public static int PlayerCount { get; private set; }

        private static readonly string[] GuestNames = new string[]
        {
            "Darq", "Deyst", "Drac", "Drol",
            "Eango", "Eashy", "Eati", "Eendi", "Ehoni",
            "Gharr", "Iatho", "Iawa", "Idrae", "Iri", "Issz", "Itani",
            "Laen", "Lauk", "Lorz",
            "Oalei", "Odaru", "Oeti", "Orothi", "Oshyu",
            "Queq", "Radph", "Rayr", "Ril", "Rilr", "Risrr",
            "Saylt", "Scheev", "Sek", "Serl", "Seus",
            "Tal", "Tiar", "Uoro", "Urake", "Utanu",
            "Vorck", "Vorv", "Yangu", "Yimi", "Zhiar"
        };

        private static DatabaseConfig _config;
        private static IDatabase _db;

        public static void Connect(DatabaseConfig config)
        {
            _config = config;

            RedisClient.Connect(config);

            _db = RedisClient.Db;
        }

        public static bool IsValidUsername(string name)
            => !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);

        public static bool IsValidPassword(string password)
            => !string.IsNullOrWhiteSpace(password) && password.Length > 8;

        public static RegisterStatus Register(string username, string password, string ip)
        {
            if (!IsValidUsername(username))
                return RegisterStatus.InvalidName;
            if (!IsValidPassword(password))
                return RegisterStatus.InvalidPassword;

            string accNames = (string)_db.StringGet(DbKeys.AccountNames);
            if (accNames != null && accNames.Contains(username.ToLower()))
            {
                return RegisterStatus.NameInUse;
            }

            int? accCount = (int?)_db.HashGet(DbKeys.AccountIPs, ip);
            if (accCount != null && accCount > _config.MaxAccountsPerIP)
            {
                return RegisterStatus.MaxAccountsReached;
            }

            var accId = (int)_db.StringIncrement(DbKeys.NextAccountId);
            var acc = new DbAccount(accId)
            {
                AccountId = accId,
                Name = username,
                MaxNumChars = NewAccountsConfig.Config.MaxChars,
                AliveChars = new int[0],
                DeadChars = new int[0],
                OwnedSkins = new int[0],
                Stats = new DbAccStats(accId)
                {
                    Credits = NewAccountsConfig.Config.Credits,
                    TotalCredits = NewAccountsConfig.Config.Credits,
                    Fame = NewAccountsConfig.Config.Fame,
                    TotalFame = NewAccountsConfig.Config.Fame,
                    ClassStats = new ClassStatsInfo[0]
                },
                RegisterTime = DateTime.Now.ToUnixTimestamp(),
                LockedIds = new int[0],
                IgnoredIds = new int[0]
            };

            var trans = _db.CreateTransaction();
            acc.Save(trans);

            if (!trans.Execute())
            {
                return RegisterStatus.InternalError;
            }

            if (accCount == null)
                accCount = 1;
            else accCount++; // Increment account count on this ip address

            accNames += $"{acc.Name}|";

            var saveTrans = _db.CreateTransaction();

            var logins = new DbLogins();
            logins.Load(_db);

            if (!logins.AddLogin(accId, username, password)) // If for whatever reason this happens..
            {
                _log.Error($"REGISTER FAULT: tried to register an account with an existing account id: {accId}");

                return RegisterStatus.InternalError;
            }

            saveTrans.StringSetAsync(DbKeys.AccountNames, accNames, flags: CommandFlags.FireAndForget);
            saveTrans.HashSetAsync(DbKeys.AccountIPs, ip, accCount, When.Always, CommandFlags.FireAndForget);
            logins.Save(saveTrans);

            saveTrans.Execute(CommandFlags.FireAndForget);

            return RegisterStatus.Success;
        }

        public static DbAccount VerifyAccount(string username, string password, out VerifyStatus status)
        {
            var logins = new DbLogins();
            logins.Load(_db);

            var loginInfo = logins.GetLogin(username);
            if (loginInfo == null)
            {
                status = VerifyStatus.InvalidCredentials;
                return null;
            }

            if (loginInfo.Hash != (password + loginInfo.Salt).ToSHA1())
            {
                status = VerifyStatus.InvalidCredentials;
                return null;
            }

            var acc = new DbAccount(loginInfo.AccId);
            acc.Load(_db);

            status = VerifyStatus.Success;
            return acc;
        }

        public static DbAccount GetGuestAccount()
        {
            var ret = new DbAccount(-1);
            ret.Name = GuestNames.RandomElement();
            ret.MaxNumChars = NewAccountsConfig.Config.MaxChars;
            ret.AliveChars = new int[0];
            ret.Stats = new DbAccStats(-1)
            {
                Credits = NewAccountsConfig.Config.Credits,
                TotalCredits = NewAccountsConfig.Config.Credits,
                Fame = NewAccountsConfig.Config.Fame,
                TotalFame = NewAccountsConfig.Config.Fame,
                ClassStats = new ClassStatsInfo[0]
            };
            ret.RegisterTime = DateTime.Now.ToUnixTimestamp();
            return ret;
        }

        public static IEnumerable<DbChar> GetChars(int accId, params int[] ids)
        {
            foreach (var id in ids)
            {
                if (_db.KeyExists($"char.{accId}.{id}"))
                {
                    var dbChar = new DbChar(accId, id);
                    dbChar.Load(_db);
                    yield return dbChar;
                }
            }
        }

        public static DbChar GetChar(int accId, int charId)
        {
            if (_db.KeyExists($"char.{accId}.{charId}"))
            {
                var dbChar = new DbChar(accId, charId);
                dbChar.Load(_db);
                return dbChar;
            }
            return null;
        }

        public static DbChar CreateCharacter(DbAccount acc, short classType, short skinType, out CharResult result)
        {
            if (acc.AliveChars.Length + 1 > acc.MaxNumChars)
            {
                result = CharResult.MaxCharactersReached;
                return null;
            }

            if (skinType != 0 && !acc.OwnedSkins.Contains(skinType))
            {
                result = CharResult.SkinNowOwned;
                return null;
            }

            var classDesc = XmlLibrary.PlayerDescs[(ushort)classType];
            var saveTrans = _db.CreateTransaction();
            var charId = acc.NextCharId + 1;
            var chr = new DbChar(acc.AccountId, charId)
            {
                Experience = NewCharsConfig.Config.Experience,
                Level = NewCharsConfig.Config.Level,
                ClassType = classType,
                HP = classDesc.StartingValues[0],
                MP = classDesc.StartingValues[1],
                Stats = classDesc.StartingValues,
                Inventory = classDesc.Equipment,
                Fame = NewCharsConfig.Config.Fame,
                Tex1 = NewCharsConfig.Config.Tex1,
                Tex2 = NewCharsConfig.Config.Tex2,
                SkinType = skinType,
                HealthPotions = NewCharsConfig.Config.HealthPotions,
                MagicPotions = NewCharsConfig.Config.MagicPotions,
                CreationTime = DateTime.Now.ToUnixTimestamp(),
                HasBackpack = NewCharsConfig.Config.HasBackpack
            };
            chr.Save(saveTrans);

            acc.NextCharId++;
            acc.AliveChars = acc.AliveChars.Append(chr.CharId).ToArray();
            acc.Save(saveTrans);

            if (saveTrans.Execute())
            {
                result = CharResult.Success;
                return chr;
            }

            result = CharResult.InternalError;
            return null;
        }

        public static bool DeleteChar(DbAccount acc, DbChar chr)
        {
            var trans = _db.CreateTransaction();

            chr.Deleted = true; // Don't set dead to true
            acc.AliveChars = acc.AliveChars.Where(i => i != chr.CharId).ToArray();
            acc.Save(trans);
            chr.Save(trans);

            return trans.Execute();
        }

        public static void SaveCharacter(DbChar chr)
        {
            var trans = _db.CreateTransaction();

            chr.Save(trans);

            trans.Execute(CommandFlags.FireAndForget);
        }
    }
}
