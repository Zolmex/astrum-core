using System;
using System.ComponentModel;

namespace Common
{
    public enum RegisterStatus
    {
        [Description("Success.")]
        Success,
        [Description("Name taken.")]
        NameInUse,
        [Description("Invalid name.")]
        InvalidName,
        [Description("Invalid password.")]
        InvalidPassword,
        [Description("Accounts limit reached.")]
        MaxAccountsReached,
        [Description("Internal server error")]
        InternalError
    }

    public enum BuyStatus
    {
        [Description("Success.")]
        Success,
        [Description("Not enough gold.")]
        NotEnoughCredits,
        [Description("Not enough fame.")]
        NotEnoughFame
    }

    public enum CharResult
    {
        [Description("Success")]
        Success,
        [Description("You don't own this skin.")]
        SkinNowOwned,
        [Description("Characters limit reached.")]
        MaxCharactersReached,
        [Description("Internal server error.")]
        InternalError
    }

    public enum VerifyStatus
    {
        [Description("Success")]
        Success,
        [Description("Invalid account credentials.")]
        InvalidCredentials
    }

    public enum StatType
    {
        MaxHP,
        HP,
        Size,
        MaxMP,
        MP,
        NextLevelExp,
        Experience,
        Level,
        Inventory0,
        Inventory1,
        Inventory2,
        Inventory3,
        Inventory4,
        Inventory5,
        Inventory6,
        Inventory7,
        Inventory8,
        Inventory9,
        Inventory10,
        Inventory11,
        Attack,
        Defense,
        Speed,
        Vitality,
        Wisdom,
        Dexterity,
        Condition,
        NumStars,
        Name,
        Tex1,
        Tex2,
        MerchandiseType,
        MerchandisePrice,
        Credits,
        Active,
        AccountId,
        Fame,
        MerchandiseCurrency,
        Connect,
        MerchandiseCount,
        MerchandiseMinsLeft,
        MerchandiseDiscount,
        MerchandiseRankReq,
        MaxHPBoost,
        MaxMPBoost,
        AttackBoost,
        DefenseBoost,
        SpeedBoost,
        VitalityBoost,
        WisdomBoost,
        DexterityBoost,
        CharFame,
        NextClassQuestFame,
        LegendaryRank,
        SinkLevel,
        AltTexture,
        GuildName,
        GuildRank,
        Oxygen,
        HealthPotionStack,
        MagicPotionStack,
        Backpack0,
        Backpack1,
        Backpack2,
        Backpack3,
        Backpack4,
        Backpack5,
        Backpack6,
        Backpack7,
        HasBackpack,
        Texture,
        InventoryData0,
        InventoryData1,
        InventoryData2,
        InventoryData3,
        InventoryData4,
        InventoryData5,
        InventoryData6,
        InventoryData7,
        InventoryData8,
        InventoryData9,
        InventoryData10,
        InventoryData11,
        InventoryData12,
        InventoryData13,
        InventoryData14,
        InventoryData15,
        InventoryData16,
        InventoryData17,
        InventoryData18,
        InventoryData19,
        
        None = int.MaxValue
    }

    public enum ItemType
    {
        All,
        Sword,
        Dagger,
        Bow,
        Tome,
        Shield,
        Leather,
        Plate,
        Wand,
        Ring,
        Potion,
        Spell,
        Seal,
        Cloak,
        Robe,
        Quiver,
        Helm,
        Staff,
        Poison,
        Skull,
        Trap,
        Orb,
        Prism,
        Scepter,
        Katana,
        Shuriken,

        Weapon = 50,
        Ability = 51,
        Armor = 52
    }

    [Flags]
    public enum ConditionEffects : ulong
    {
        Nothing = 1 << 0,
        Quiet = 1 << 1,
        Weak = 1 << 2,
        Slowed = 1 << 3,
        Sick = 1 << 4,
        Dazed = 1 << 5,
        Stunned = 1 << 6,
        Blind = 1 << 7,
        Hallucinating = 1 << 8,
        Drunk = 1 << 9,
        Confused = 1 << 10,
        StunImmume = 1 << 11,
        Invisible = 1 << 12,
        Paralyzed = 1 << 13,
        Speedy = 1 << 14,
        Bleeding = 1 << 15,
        Healing = 1 << 16,
        Damaging = 1 << 17,
        Berserk = 1 << 18,
        Stasis = 1 << 19,
        StasisImmune = 1 << 20,
        Invincible = 1 << 21,
        Invulnerable = 1 << 23,
        Armored = 1 << 24,
        ArmorBroken = 1 << 25,
        Hexed = 1 << 26,
        NinjaSpeedy = 1 << 27,
    }

    public enum ConditionEffectIndex
    {
        Nothing = 0,
        Quiet = 1,
        Weak = 2,
        Slowed = 3,
        Sick = 4,
        Dazed = 5,
        Stunned = 6,
        Blind = 7,
        Hallucinating = 8,
        Drunk = 9,
        Confused = 10,
        StunImmune = 11,
        Invisible = 12,
        Paralyzed = 13,
        Speedy = 14,
        Bleeding = 15,
        Healing = 16,
        Damaging = 17,
        Berserk = 18,
        Stasis = 19,
        StasisImmune = 20,
        Invincible = 21,
        Invulnerable = 22,
        Armored = 23,
        ArmorBroken = 24,
        Hexed = 25,
    }

    public enum ActivateEffectIndex
    {
        Create,
        Dye,
        Shoot,
        IncrementStat,
        Heal,
        Magic,
        HealNova,
        StatBoostSelf,
        StatBoostAura,
        BulletNova,
        ConditionEffectSelf,
        ConditionEffectAura,
        Teleport,
        PoisonGrenade,
        VampireBlast,
        Trap,
        StasisBlast,
        Pet,
        Decoy,
        Lightning,
        UnlockPortal,
        MagicNova,
        ClearConditionEffectAura,
        RemoveNegativeConditions,
        ClearConditionEffectSelf,
        ClearConditionsEffectSelf,
        RemoveNegativeConditionsSelf,
        Shuriken,
        DazeBlast,
        Backpack,
        PermaPet
    }

    public enum ShowEffectIndex
    {
        Unknown = 0,
        Heal = 1,
        Teleport = 2,
        Stream = 3,
        Throw = 4,
        Nova = 5,
        Poison = 6,
        Line = 7,
        Burst = 8,
        Flow = 9,
        Ring = 10,
        Lightning = 11,
        Collapse = 12,
        Coneblast = 13,
        Jitter = 14,
        Flash = 15,
        ThrowProjectile = 16
    }

    public enum TerrainType
    {
        None,
        Mountains,
        HighSand,
        HighPlains,
        HighForest,
        MidSand,
        MidPlains,
        MidForest,
        LowSand,
        LowPlains,
        LowForest,
        ShoreSand,
        ShorePlains,
        BeachTowels
    }
}
