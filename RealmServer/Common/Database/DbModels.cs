using Common.Resources.Config;
using Common.Utilities;
using Common.Utilities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common.Database
{
    public abstract class DbObject
    {
        protected readonly DbWriter _writer = new DbWriter();
        protected readonly DbReader _reader = new DbReader();

        public abstract void Load(IDatabase db);
        public abstract void Save(ITransaction trans);
    }

    public class DbAccount : DbObject
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int NextCharId { get; set; }
        public int MaxNumChars { get; set; }
        public int NumStars { get; set; }
        public int[] AliveChars { get; set; }
        public int[] DeadChars { get; set; }
        public int[] OwnedSkins { get; set; }
        public bool Admin { get; set; }
        public int Rank { get; set; }
        public bool Muted { get; set; }
        public bool Banned { get; set; }
        public string GuildName { get; set; }
        public int GuildRank { get; set; }
        public DbAccStats Stats { get; set; }
        public int RegisterTime { get; set; }
        public int[] LockedIds { get; set; }
        public int[] IgnoredIds { get; set; }

        public DbAccount(int accId)
        {
            AccountId = accId;
            Stats = new DbAccStats(accId);
        }

        public override void Load(IDatabase db)
        {
            string key = $"account.{AccountId}";
            Name = (string)db.HashGet(key, "name");
            NextCharId = (int)db.HashGet(key, "nextCharId");
            MaxNumChars = (int)db.HashGet(key, "maxChars");
            NumStars = (int)db.HashGet(key, "numStars");
            AliveChars = _reader.ReadIntArray(db.HashGet(key, "aliveChars"));
            DeadChars = _reader.ReadIntArray(db.HashGet(key, "deadChars"));
            Admin = (bool)db.HashGet(key, "admin");
            Rank = (int)db.HashGet(key, "rank");
            Muted = (bool)db.HashGet(key, "muted");
            Banned = (bool)db.HashGet(key, "banned");
            GuildName = (string)db.HashGet(key, "guildName");
            GuildRank = (int)db.HashGet(key, "guildRank");
            Stats.Load(db);
            RegisterTime = (int)db.HashGet(key, "registerTime");
            LockedIds = _reader.ReadIntArray(db.HashGet(key, "LockedIds"));
            IgnoredIds = _reader.ReadIntArray(db.HashGet(key, "IgnoredIds"));
        }

        public override void Save(ITransaction trans)
        {
            string key = $"account.{AccountId}";
            trans.HashSetAsync(key, "name", Name);
            trans.HashSetAsync(key, "nextCharId", NextCharId);
            trans.HashSetAsync(key, "maxChars", MaxNumChars);
            trans.HashSetAsync(key, "numStars", NumStars);
            trans.HashSetAsync(key, "aliveChars", _writer.Write(AliveChars));
            trans.HashSetAsync(key, "deadChars", _writer.Write(DeadChars));
            trans.HashSetAsync(key, "ownedSkins", _writer.Write(OwnedSkins));
            trans.HashSetAsync(key, "admin", Admin);
            trans.HashSetAsync(key, "rank", Rank);
            trans.HashSetAsync(key, "muted", Muted);
            trans.HashSetAsync(key, "banned", Banned);
            trans.HashSetAsync(key, "guildName", GuildName);
            trans.HashSetAsync(key, "guildRank", GuildRank);
            Stats.Save(trans);
            trans.HashSetAsync(key, "registerTime", RegisterTime);
            trans.HashSetAsync(key, "lockedIds", _writer.Write(LockedIds));
            trans.HashSetAsync(key, "ignoredIds", _writer.Write(IgnoredIds));
        }

        public XElement ToXml()
           => new XElement("Account",
               new XElement("AccountId", AccountId),
               new XElement("Name", Name),
               new XElement("Guild",
                   new XElement("Name", GuildName),
                   new XElement("Rank", GuildRank)),
               Admin ? new XElement("Admin") : null,
               Stats.ToXml()
               );

        public XElement ToCharListXml()
            => new XElement("Chars",
                    new XAttribute("nextCharId", NextCharId),
                    new XAttribute("maxNumChars", MaxNumChars),
                    new XAttribute("charSlotCost", NewAccountsConfig.Config.CharSlotCost),
                    new XElement("OwnedSkins", OwnedSkins.ToCommaSepString(",")),
                    ToXml(),
                    DbClient.GetChars(AccountId, AliveChars).Select(c => c.ToXml()),
                    NewsConfig.Config.Models.Select(n => n.ToXml()),
                    new XElement("Servers",
                        new XElement("Server",
                            new XElement("Name", GameServerConfig.Config.ServerName),
                            new XElement("DNS", GameServerConfig.Config.Address),
                            new XElement("Port", GameServerConfig.Config.Port),
                            new XElement("Players", DbClient.PlayerCount),
                            new XElement("MaxPlayers", GameServerConfig.Config.MaxPlayers),
                            new XElement("AdminOnly", GameServerConfig.Config.AdminOnly ? "true" : "false")
                            )
                    )
                    );
    }

    public class DbAccStats : DbObject
    {
        public int AccId { get; set; }
        public int BestCharFame { get; set; }
        public int TotalFame { get; set; }
        public int Fame { get; set; }
        public int Credits { get; set; }
        public int TotalCredits { get; set; }
        public ClassStatsInfo[] ClassStats { get; set; }

        public DbAccStats(int accId)
        {
            AccId = accId;
        }

        public ClassStatsInfo GetClassStats(int type)
            => ClassStats.FirstOrDefault(s => s.ObjectType == type);

        public override void Load(IDatabase db)
        {
            var key = $"account.{AccId}";
            BestCharFame = (int)db.HashGet(key, "bestCharFame");
            TotalFame = (int)db.HashGet(key, "totalFame");
            Fame = (int)db.HashGet(key, "fame");
            Credits = (int)db.HashGet(key, "credits");
            TotalCredits = (int)db.HashGet(key, "totalCredits");
            ClassStats = JsonConvert.DeserializeObject<ClassStatsInfo[]>((string)db.HashGet(key, "classStats"));
        }

        public override void Save(ITransaction trans)
        {
            var key = $"account.{AccId}";
            trans.HashSetAsync(key, "bestCharFame", BestCharFame);
            trans.HashSetAsync(key, "totalFame", TotalFame);
            trans.HashSetAsync(key, "fame", Fame);
            trans.HashSetAsync(key, "credits", Credits);
            trans.HashSetAsync(key, "totalCredits", TotalCredits);
            trans.HashSetAsync(key, "classStats", JsonConvert.SerializeObject(ClassStats));
        }

        public XElement ToXml()
            => new XElement("Stats",
                new XElement("BestCharFame", BestCharFame),
                new XElement("TotalFame", TotalFame),
                new XElement("Fame", Fame),
                new XElement("TotalCredits", TotalCredits),
                new XElement("Credits", Credits),
                ClassStats.Select(s => s.ToXml())
                );
    }

    public class ClassStatsInfo
    {
        public int ObjectType { get; set; }
        public int BestLevel { get; set; }
        public int BestFame { get; set; }

        public XElement ToXml()
            => new XElement("ClassStats",
                new XAttribute("objectType", ObjectType),
                new XElement("BestLevel", BestLevel),
                new XElement("BestFame", BestFame)
                );
    }

    public class DbLogins : DbObject
    {
        public Dictionary<string, AccountLogin> Logins { get; set; }

        public override void Load(IDatabase db)
        {
            Logins = new Dictionary<string, AccountLogin>();

            foreach (var hash in db.HashGetAll(DbKeys.AccountLogins))
            {
                var name = (string)hash.Name;
                var login = JsonConvert.DeserializeObject<AccountLogin>(hash.Value);
                Logins[name] = login;
            }
        }

        public override void Save(ITransaction trans)
        {
            foreach (var kvp in Logins)
                trans.HashSetAsync(DbKeys.AccountLogins, kvp.Key, JsonConvert.SerializeObject(kvp.Value));
        }

        public bool AddLogin(int accId, string username, string password)
        {
            var salt = MathUtils.GenerateSalt();
            var login = new AccountLogin();
            login.AccId = accId;
            login.Name = username;
            login.Hash = (password + salt).ToSHA1();
            login.Salt = salt;

            return Logins.TryAdd(username, login);
        }

        public AccountLogin GetLogin(string username)
        {
            if (!Logins.TryGetValue(username, out var ret))
                return null;
            return ret;
        }
    }

    public class AccountLogin
    {
        public int AccId { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }

    public class DbChar : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int ClassType { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int[] Stats { get; set; }
        public int[] Inventory { get; set; }
        public int Fame { get; set; }
        public int Tex1 { get; set; }
        public int Tex2 { get; set; }
        public int SkinType { get; set; }
        public int HealthPotions { get; set; }
        public int MagicPotions { get; set; }
        public int CreationTime { get; set; }
        public bool Deleted { get; set; }
        public bool Dead { get; set; }
        public int DeathFame { get; set; }
        public int DeathTime { get; set; }
        public bool HasBackpack { get; set; }
        public DbFameStats FameStats { get; set; }
        public int PetId { get; set; }

        public DbChar(int accId, int charId)
        {
            CharId = charId;
            AccId = accId;
            FameStats = new DbFameStats(accId, charId);
        }

        public override void Load(IDatabase db)
        {
            var key = $"char.{AccId}.{CharId}";
            Experience = (int)db.HashGet(key, "experience");
            Level = (int)db.HashGet(key, "level");
            ClassType = (int)db.HashGet(key, "classType");
            HP = (int)db.HashGet(key, "hp");
            MP = (int)db.HashGet(key, "mp");
            Stats = _reader.ReadIntArray(db.HashGet(key, "stats"));
            Inventory = _reader.ReadIntArray(db.HashGet(key, "inventory"));
            Fame = (int)db.HashGet(key, "fame");
            Tex1 = (int)db.HashGet(key, "tex1");
            Tex2 = (int)db.HashGet(key, "tex2");
            SkinType = (int)db.HashGet(key, "skinType");
            HealthPotions = (int)db.HashGet(key, "healthPotions");
            MagicPotions = (int)db.HashGet(key, "magicPotions");
            CreationTime = (int)db.HashGet(key, "creationTime");
            Deleted = (bool)db.HashGet(key, "deleted");
            Dead = (bool)db.HashGet(key, "dead");
            DeathFame = (int)db.HashGet(key, "deathFame");
            DeathTime = (int)db.HashGet(key, "deathTime");
            HasBackpack = (bool)db.HashGet(key, "hasBackpack");
            PetId = (int)db.HashGet(key, "petId");
        }

        public override void Save(ITransaction trans)
        {
            var key = $"char.{AccId}.{CharId}";
            trans.HashSetAsync(key, "experience", Experience);
            trans.HashSetAsync(key, "level", Level);
            trans.HashSetAsync(key, "classType", ClassType);
            trans.HashSetAsync(key, "hp", HP);
            trans.HashSetAsync(key, "mp", MP);
            trans.HashSetAsync(key, "stats", _writer.Write(Stats));
            trans.HashSetAsync(key, "inventory", _writer.Write(Inventory));
            trans.HashSetAsync(key, "fame", Fame);
            trans.HashSetAsync(key, "tex1", Tex1);
            trans.HashSetAsync(key, "tex2", Tex2);
            trans.HashSetAsync(key, "skinType", SkinType);
            trans.HashSetAsync(key, "healthPotions", HealthPotions);
            trans.HashSetAsync(key, "magicPotions", MagicPotions);
            trans.HashSetAsync(key, "creationTime", CreationTime);
            trans.HashSetAsync(key, "deleted", Deleted);
            trans.HashSetAsync(key, "dead", Dead);
            trans.HashSetAsync(key, "deathFame", DeathFame);
            trans.HashSetAsync(key, "deathTime", DeathTime);
            trans.HashSetAsync(key, "hasBackpack", HasBackpack);
            trans.HashSetAsync(key, "petId", PetId);
        }

        public XElement ToXml()
            => new XElement("Char",
                new XAttribute("id", CharId),
                new XElement("ObjectType", ClassType),
                new XElement("Level", Level),
                new XElement("Exp", Experience),
                new XElement("CurrentFame", Fame),
                new XElement("Equipment", Inventory.ToCommaSepString(",")),
                new XElement("MaxHitPoints", Stats[0]),
                new XElement("HitPoints", HP),
                new XElement("MaxMagicPoints", Stats[1]),
                new XElement("MagicPoints", MP),
                new XElement("Attack", Stats[2]),
                new XElement("Defense", Stats[3]),
                new XElement("Speed", Stats[4]),
                new XElement("Dexterity", Stats[5]),
                new XElement("HpRegen", Stats[6]),
                new XElement("MpRegen", Stats[7]),
                new XElement("Tex1", Tex1),
                new XElement("Tex2", Tex2),
                new XElement("Texture", SkinType)
                );
    }

    public class DbFameStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public DbExplorationStats Exploration { get; set; }
        public DbCombatStats Combat { get; set; }
        public DbKillStats Kills { get; set; }
        public DbDungeonStats Dungeons { get; set; }

        public DbFameStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;

            Exploration = new DbExplorationStats(accId, charId);
            Combat = new DbCombatStats(accId, charId);
            Kills = new DbKillStats(accId, charId);
            Dungeons = new DbDungeonStats(accId, charId);
        }

        public override void Load(IDatabase db)
        {
            Exploration.Load(db);
            Combat.Load(db);
            Kills.Load(db);
            Dungeons.Load(db);
        }

        public override void Save(ITransaction trans)
        {
            Exploration.Save(trans);
            Combat.Save(trans);
            Kills.Save(trans);
            Dungeons.Save(trans);
        }
    }

    public class DbExplorationStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int TilesUncovered { get; set; }
        public int QuestsCompleted { get; set; }
        public int Escapes { get; set; }
        public int NearDeathEscapes { get; set; }
        public int MinutesActive { get; set; }
        public int Teleports { get; set; }

        public DbExplorationStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
        }

        public override void Load(IDatabase db)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            TilesUncovered = (int)db.HashGet(key, "tilesUncovered");
            QuestsCompleted = (int)db.HashGet(key, "questsCompleted");
            Escapes = (int)db.HashGet(key, "escapes");
            NearDeathEscapes = (int)db.HashGet(key, "nearDeathEscapes");
            MinutesActive = (int)db.HashGet(key, "minutesActive");
            Teleports = (int)db.HashGet(key, "teleports");
        }

        public override void Save(ITransaction trans)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            trans.HashSetAsync(key, "tilesUncovered", TilesUncovered);
            trans.HashSetAsync(key, "questsCompleted", QuestsCompleted);
            trans.HashSetAsync(key, "escapes", Escapes);
            trans.HashSetAsync(key, "nearDeathEscapes", NearDeathEscapes);
            trans.HashSetAsync(key, "minutesActive", MinutesActive);
            trans.HashSetAsync(key, "teleports", Teleports);
        }
    }

    public class DbCombatStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int Shots { get; set; }
        public int ShotsThatDamage { get; set; }
        public int LevelUpAssists { get; set; }
        public int PotionsDrank { get; set; }
        public int AbilitiesUsed { get; set; }
        public int DamageTaken { get; set; }
        public int DamageDealt { get; set; }

        public DbCombatStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
        }

        public override void Load(IDatabase db)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            Shots = (int)db.HashGet(key, "shots");
            ShotsThatDamage = (int)db.HashGet(key, "shotsThatDamage");
            LevelUpAssists = (int)db.HashGet(key, "levelUpAssists");
            PotionsDrank = (int)db.HashGet(key, "potionsDrank");
            AbilitiesUsed = (int)db.HashGet(key, "abilitiesUsed");
            DamageTaken = (int)db.HashGet(key, "damageTaken");
            DamageDealt = (int)db.HashGet(key, "damageDealt");
        }

        public override void Save(ITransaction trans)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            trans.HashSetAsync(key, "shots", Shots);
            trans.HashSetAsync(key, "shotsThatDamage", ShotsThatDamage);
            trans.HashSetAsync(key, "levelUpAssists", LevelUpAssists);
            trans.HashSetAsync(key, "potionsDrank", PotionsDrank);
            trans.HashSetAsync(key, "abilitiesUsed", AbilitiesUsed);
            trans.HashSetAsync(key, "damageTaken", DamageTaken);
            trans.HashSetAsync(key, "damageDealt", DamageDealt);
        }
    }

    public class DbKillStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int MonsterKills { get; set; }
        public int MonsterAssists { get; set; }
        public int GodKills { get; set; }
        public int GodAssists { get; set; }
        public int OryxKills { get; set; }
        public int OryxAssists { get; set; }
        public int CubeKills { get; set; }
        public int CubeAssists { get; set; }
        public int BlueBags { get; set; }
        public int CyanBags { get; set; }
        public int WhiteBags { get; set; }

        public DbKillStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
        }

        public override void Load(IDatabase db)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            MonsterKills = (int)db.HashGet(key, "monsterKills");
            MonsterAssists = (int)db.HashGet(key, "monsterAssists");
            GodKills = (int)db.HashGet(key, "godKills");
            GodAssists = (int)db.HashGet(key, "godAssists");
            OryxKills = (int)db.HashGet(key, "oryxKills");
            OryxAssists = (int)db.HashGet(key, "oryxAssists");
            CubeKills = (int)db.HashGet(key, "cubeKills");
            CubeAssists = (int)db.HashGet(key, "cubeAssists");
            BlueBags = (int)db.HashGet(key, "blueBags");
            CyanBags = (int)db.HashGet(key, "cyanBags");
            WhiteBags = (int)db.HashGet(key, "whiteBags");
        }

        public override void Save(ITransaction trans)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            trans.HashSetAsync(key, "monsterKills", MonsterKills);
            trans.HashSetAsync(key, "monsterAssists", MonsterAssists);
            trans.HashSetAsync(key, "godKills", GodKills);
            trans.HashSetAsync(key, "godAssists", GodAssists);
            trans.HashSetAsync(key, "oryxKills", OryxKills);
            trans.HashSetAsync(key, "oryxAssists", OryxAssists);
            trans.HashSetAsync(key, "cubeKills", CubeKills);
            trans.HashSetAsync(key, "cubeAssists", CubeAssists);
            trans.HashSetAsync(key, "blueBags", BlueBags);
            trans.HashSetAsync(key, "cyanBags", CyanBags);
            trans.HashSetAsync(key, "whiteBags", WhiteBags);
        }
    }

    public class DbDungeonStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int PirateCavesCompleted { get; set; }
        public int UndeadLairsCompleted { get; set; }
        public int AbyssOfDemonsCompleted { get; set; }
        public int SnakePitsCompleted { get; set; }
        public int SpiderDensCompleted { get; set; }
        public int SpriteWorldsCompleted { get; set; }
        public int TombsCompleted { get; set; }

        public DbDungeonStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
        }

        public override void Load(IDatabase db)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            PirateCavesCompleted = (int)db.HashGet(key, "pirateCavesCompleted");
            UndeadLairsCompleted = (int)db.HashGet(key, "undeadLairsCompleted");
            AbyssOfDemonsCompleted = (int)db.HashGet(key, "abyssOfDemonsCompleted");
            SnakePitsCompleted = (int)db.HashGet(key, "snakePitsCompleted");
            SpiderDensCompleted = (int)db.HashGet(key, "spiderDensCompleted");
            SpriteWorldsCompleted = (int)db.HashGet(key, "spriteWorldsCompleted");
            TombsCompleted = (int)db.HashGet(key, "tombsCompleted");
        }

        public override void Save(ITransaction trans)
        {
            var key = $"charExploration.{AccId}.{CharId}";
            trans.HashSetAsync(key, "pirateCavesCompleted", PirateCavesCompleted);
            trans.HashSetAsync(key, "undeadLairsCompleted", UndeadLairsCompleted);
            trans.HashSetAsync(key, "abyssOfDemonsCompleted", AbyssOfDemonsCompleted);
            trans.HashSetAsync(key, "snakePitsCompleted", SnakePitsCompleted);
            trans.HashSetAsync(key, "spiderDensCompleted", SpiderDensCompleted);
            trans.HashSetAsync(key, "spriteWorldsCompleted", SpriteWorldsCompleted);
            trans.HashSetAsync(key, "tombsCompleted", TombsCompleted);
        }
    }
}
