using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Common.Resources.Xml
{
    public class ObjectDesc
    {
        public readonly XElement XML;
        public readonly string ObjectId;
        public readonly ushort ObjectType;

        public readonly string DisplayId;
        public readonly string DisplayName;
        public readonly string Class;

        public readonly bool Static;
        public readonly bool CaveWall;
        public readonly bool ConnectedWall;
        public readonly bool BlocksSight;

        public readonly bool OccupySquare;
        public readonly bool FullOccupy;
        public readonly bool EnemyOccupySquare;

        public readonly bool ProtectFromGroundDamage;
        public readonly bool ProtectFromSink;

        public readonly bool Player;
        public readonly bool Enemy;

        public readonly bool God;
        public readonly bool Cube;
        public readonly bool Quest;
        public readonly bool Hero;
        public readonly int Level;
        public readonly bool Oryx;
        public readonly float XpMult;

        public readonly int Size;
        public readonly int MinSize;
        public readonly int MaxSize;

        public readonly int MaxHP;
        public readonly int Defense;

        public readonly string DungeonName;
        public readonly bool RealmPortal;

        public readonly Dictionary<int, ProjectileDesc> Projectiles;
        public readonly LootTableDesc LootTable;
        public readonly StateDesc BehaviorState;
        public readonly bool ActiveBehavior;

        public ObjectDesc(XElement e, string id, ushort type)
        {
            XML = e;
            ObjectId = id;
            ObjectType = type;

            DisplayId = e.GetValue<string>("DisplayId", ObjectId);
            DisplayName = DisplayId ?? ObjectId;
            Class = e.GetValue<string>("Class");

            Static = e.HasElement("Static");
            CaveWall = Class == "CaveWall";
            ConnectedWall = Class == "ConnectedWall";
            BlocksSight = e.HasElement("BlocksSight");

            OccupySquare = e.HasElement("OccupySquare");
            FullOccupy = e.HasElement("FullOccupy");
            EnemyOccupySquare = e.HasElement("EnemyOccupySquare");

            ProtectFromGroundDamage = e.HasElement("ProtectFromGroundDamage");
            ProtectFromSink = e.HasElement("ProtectFromSink");

            Enemy = e.HasElement("Enemy");
            Player = e.HasElement("Player");

            God = e.HasElement("God");
            Cube = e.HasElement("Cube");
            Quest = e.HasElement("Quest");
            Hero = e.HasElement("Hero");
            Level = e.GetValue<int>("Level", -1);
            Oryx = e.HasElement("Oryx");
            XpMult = e.GetValue<float>("XpMult", 1);

            Size = e.GetValue<int>("Size", 100);
            MinSize = e.GetValue<int>("MinSize", Size);
            MaxSize = e.GetValue<int>("MaxSize", Size);

            MaxHP = e.GetValue<int>("MaxHitPoints", 100);
            Defense = e.GetValue<int>("Defense");

            DungeonName = e.GetValue<string>("DungeonName");
            RealmPortal = e.GetValue<bool>("RealmPortal");

            Projectiles = new Dictionary<int, ProjectileDesc>();
            foreach (XElement k in e.Elements("Projectile"))
            {
                ProjectileDesc desc = new ProjectileDesc(k, ObjectType);
                Projectiles[desc.BulletType] = desc;
            }

            BehaviorState = e.HasElement("State") ? new StateDesc(e.Element("State")) : null;
            LootTable = e.HasElement("LootTable") ? new LootTableDesc(e.Element("LootTable")) : null;
            ActiveBehavior = e.HasElement("ActiveBehavior");
        }
    }

    public class PlayerDesc : ObjectDesc
    {
        public readonly int[] SlotTypes;
        public readonly int[] Equipment;
        public readonly StatDesc[] Stats;
        public readonly int[] StartingValues;

        public PlayerDesc(XElement e, string id, ushort type)
            : base(e, id, type)
        {
            SlotTypes = e.GetValue<string>("SlotTypes")?.CommaToArray<int>();
            Equipment = e.GetValue<string>("Equipment")?.CommaToArray<int>();
            Stats = new StatDesc[8];
            for (int i = 0; i < Stats.Length; i++)
                Stats[i] = new StatDesc(i, e);
            Stats = Stats.OrderBy(k => k.Index).ToArray();

            StartingValues = Stats.Select(k => k.StartingValue).ToArray();
        }
    }

    public class StatDesc
    {
        public readonly string Type;
        public readonly int Index;
        public readonly int MaxValue;
        public readonly int StartingValue;
        public readonly int MinIncrease;
        public readonly int MaxIncrease;

        public StatDesc(int index, XElement e)
        {
            Index = index;
            Type = GameUtils.StatIndexToName(index);

            StartingValue = e.GetValue<int>(Type);
            MaxValue = e.Element(Type).GetAttribute<int>("max");

            var stat = e.Elements("LevelIncrease").FirstOrDefault(s => s.Value == Type);
            if (stat != null)
            {
                MinIncrease = stat.GetAttribute<int>("min");
                MaxIncrease = stat.GetAttribute<int>("max");
            }
        }
    }

    public class SkinDesc
    {
        public readonly string Id;
        public readonly ushort Type;

        public readonly ushort PlayerClassType;

        public SkinDesc(XElement e, string id, ushort type)
        {
            Id = id;
            Type = type;
            PlayerClassType = e.GetValue<ushort>("PlayerClassType");
        }
    }

    [DataContract]
    public class ActivateEffectDesc
    {
        public readonly ActivateEffectIndex Index;
        public readonly ConditionEffectDesc[] Effects;
        public readonly ConditionEffectIndex Effect;
        [DataMember] public int DurationMS;
        [DataMember] public float Range;
        [DataMember] public int Amount;
        [DataMember] public int TotalDamage;
        [DataMember] public float Radius;
        [DataMember] public uint? Color;
        [DataMember] public int MaxTargets;

        [JsonConstructor]
        public ActivateEffectDesc() { }

        public ActivateEffectDesc(XElement e)
        {
            Index = (ActivateEffectIndex)Enum.Parse(typeof(ActivateEffectIndex), e.Value.Replace(" ", ""));
            Effect = Utils.GetEnumValue<ConditionEffectIndex>(e.GetAttribute<string>("effect"));
            DurationMS = (int)(e.GetAttribute<float>("duration", 0) * 1000);
            Range = e.GetAttribute<float>("range");
            Amount = e.GetAttribute<int>("amount");
            TotalDamage = e.GetAttribute<int>("totalDamage");
            Radius = e.GetAttribute<float>("radius");
            MaxTargets = e.GetAttribute<int>("maxTargets");

            Effects = new ConditionEffectDesc[1]
            {
                new ConditionEffectDesc(Effect, DurationMS)
            };

            Color = e.GetAttribute<uint>("color");
        }
    }

    [DataContract]
    public class Item
    {
        public static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public readonly XElement Root;
        public readonly string ObjectId;
        public readonly ushort ObjectType;
        public readonly int SlotType;
        public readonly bool Usable;
        public readonly int BagType;
        public readonly bool Consumable;
        public readonly bool Potion;
        public readonly bool Soulbound;
        public readonly int Tex1;
        public readonly int Tex2;

        public string DisplayName => DisplayId ?? ObjectId;
        [DataMember] public int Tier { get => GetValue<int>("Tier"); set => SetValue("Tier", value); }
        [DataMember] public string Description { get => GetValue<string>("Description"); set => SetValue("Description", value); }
        [DataMember] public float RateOfFire { get => GetValue<float>("RateOfFire"); set => SetValue("RateOfFire", value); }
        [DataMember] public int MpCost { get => GetValue<int>("MpCost"); set => SetValue("MpCost", value); }
        [DataMember] public int FameBonus { get => GetValue<int>("FameBonus"); set => SetValue("FameBonus", value); }
        [DataMember] public byte NumProjectiles { get => GetValue<byte>("NumProjectiles"); set => SetValue("NumProjectiles", value); }
        [DataMember] public float ArcGap { get => GetValue<float>("ArcGap"); set => SetValue("ArcGap", value); }
        [DataMember] public string DisplayId { get => GetValue<string>("DisplayId"); set => SetValue("DisplayId", value); }
        [DataMember] public int Cooldown { get => GetValue<int>("Cooldown"); set => SetValue("Cooldown", value); }
        [DataMember] public bool Resurrects { get => GetValue<bool>("Resurrects"); set => SetValue("Resurrects", value); }
        [DataMember] public int Doses { get => GetValue<int>("Doses"); set => SetValue("Doses", value); }
        [DataMember] public int MaxDoses { get => GetValue<int>("MaxDoses"); set => SetValue("MaxDoses", value); }
        [DataMember] public KeyValuePair<int, int>[] StatBoosts { get => GetValue<KeyValuePair<int, int>[]>("StatBoosts"); set => SetValue("StatBoosts", value); }
        [DataMember] public ActivateEffectDesc[] ActivateEffects { get => GetValue<ActivateEffectDesc[]>("ActivateEffects"); set => SetValue("ActivateEffects", value); }
        [DataMember] public ProjectileDesc Projectile { get => GetValue<ProjectileDesc>("Projectile"); set => SetValue("Projectile", value); }

        private readonly Dictionary<string, object> _original = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _changed = new Dictionary<string, object>();

        [JsonConstructor]
        public Item() { }

        public Item(XElement e, string id, ushort type)
        {
            Root = e;
            ObjectId = id;
            ObjectType = type;
            SlotType = e.GetValue<int>("SlotType");
            Usable = e.HasElement("Usable");
            BagType = e.GetValue<int>("BagType");
            Consumable = e.HasElement("Consumable");
            Potion = e.HasElement("Potion");
            Soulbound = e.HasElement("Soulbound");
            Tex1 = (int)e.GetValue<uint>("Tex1", 0);
            Tex2 = (int)e.GetValue<uint>("Tex2", 0);
            Tier = e.GetValue<int>("Tier", -1);
            Description = e.GetValue<string>("Description");
            RateOfFire = e.GetValue<float>("RateOfFire", 1);
            MpCost = e.GetValue<int>("MpCost");
            FameBonus = e.GetValue<int>("FameBonus");
            NumProjectiles = (byte)e.GetValue<int>("NumProjectiles", 1);
            ArcGap = e.GetValue<float>("ArcGap", 11.25f);
            DisplayId = e.GetValue<string>("DisplayId");
            Doses = e.GetValue<int>("Doses");
            Cooldown = (int)(e.GetValue<float>("Cooldown", 0.5f) * 1000);
            Resurrects = e.HasElement("Resurrects");
            MaxDoses = e.GetValue<int>("Doses");

            StatBoosts = e.Elements("ActivateOnEquip")
                .Select(i => new KeyValuePair<int, int>(i.GetAttribute<int>("stat"), i.GetAttribute<int>("amount")))
                .ToArray();
            ActivateEffects = e.Elements("Activate")
                .Select(i => new ActivateEffectDesc(i))
                .ToArray();
            Projectile = e.HasElement("Projectile") ? new ProjectileDesc(e.Element("Projectile"), ObjectType) : null;
        }

        private T GetValue<T>(string key, T def = default)
        {
            if (_changed.TryGetValue(key, out var value))
                return (T)(value ?? def);
            if (_original.TryGetValue(key, out value))
                return (T)(value ?? def);
            return def;
        }

        private void SetValue(string key, object value)
        {
            if (!_original.ContainsKey(key))
                _original[key] = value;
            else _changed[key] = value;
        }

        public static Item Import(Item desc, string json)
        {
            var ret = new Item(desc.Root, desc.ObjectId, desc.ObjectType);
            var itemData = JsonConvert.DeserializeObject<Item>(json, SerializeSettings);
            var props = typeof(Item).GetProperties();
            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                if (prop.CanWrite)
                {
                    var value = prop.GetValue(itemData);
                    if (!Utils.IsDefaultValue(prop.PropertyType, value))
                        prop.SetValue(ret, value);
                }
            }
            return ret;
        }

        public string Export()
        {
            if (!_changed.Any())
                return null;

            var clone = Clone();
            var props = GetType().GetProperties();
            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                if (prop.CanWrite)
                {
                    object value = null;
                    if (_changed.ContainsKey(prop.Name))
                        value = _changed[prop.Name];
                    prop.SetValue(clone, value);
                }
            }
            var ret = JsonConvert.SerializeObject(clone, SerializeSettings);
            return ret;
        }

        public Item Clone()
            => new Item(Root, ObjectId, ObjectType);

        public static bool TypeEquals(int slotType, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.All: return true;
                case ItemType.Weapon:
                    return slotType == 1 || slotType == 2 || slotType == 3 || slotType == 8 || slotType == 17 || slotType == 24;
                case ItemType.Ability:
                    return slotType == 4 || slotType == 5 || slotType == 11 || slotType == 12 || slotType == 13 || slotType == 15 || slotType == 16 || slotType == 18 || slotType == 19 || slotType == 20 || slotType == 21 || slotType == 22 || slotType == 23 || slotType == 25;
                case ItemType.Armor:
                    return slotType == 6 || slotType == 7 || slotType == 14;
                default:
                    return slotType == (int)itemType;
            }
        }
    }

    public class TileDesc
    {
        public readonly string ObjectId;
        public readonly ushort ObjectType;
        public readonly bool NoWalk;
        public readonly int Damage;
        public readonly float Speed;
        public readonly bool Sinking;
        public readonly bool Push;
        public readonly float DX;
        public readonly float DY;

        public TileDesc(XElement e, string id, ushort type)
        {
            ObjectId = id;
            ObjectType = type;
            NoWalk = e.HasElement("NoWalk");
            Damage = e.GetValue<int>("Damage");
            Speed = e.GetValue<float>("Speed", 1.0f);
            Sinking = e.HasElement("Sinking");
            if (Push = e.HasElement("Push"))
            {
                DX = e.Element("Animate").GetAttribute<float>("dx") / 1000f;
                DY = e.Element("Animate").GetAttribute<float>("dy") / 1000f;
            }
        }
    }

    [DataContract]
    public class ProjectileDesc
    {
        public readonly ushort ContainerType;
        public readonly byte BulletType;
        public readonly string ObjectId;
        [DataMember] public int LifetimeMS;
        [DataMember] public float Speed;

        [DataMember] public int Damage;
        [DataMember] public int MinDamage;
        [DataMember] public int MaxDamage;

        public readonly ConditionEffectDesc[] Effects;

        [DataMember] public bool MultiHit;
        [DataMember] public bool PassesCover;
        [DataMember] public bool ArmorPiercing;
        [DataMember] public bool Wavy;
        [DataMember] public bool Parametric;
        [DataMember] public bool Boomerang;

        [DataMember] public float Amplitude;
        [DataMember] public float Frequency;
        [DataMember] public float Magnitude;

        [JsonConstructor]
        public ProjectileDesc() { }

        public ProjectileDesc(XElement e, ushort containerType)
        {
            ContainerType = containerType;
            BulletType = (byte)e.GetAttribute<int>("id");
            ObjectId = e.GetValue<string>("ObjectId");
            LifetimeMS = e.GetValue<int>("LifetimeMS");
            Speed = e.GetValue<float>("Speed");
            Damage = e.GetValue<int>("Damage");
            MinDamage = e.GetValue<int>("MinDamage", Damage);
            MaxDamage = e.GetValue<int>("MaxDamage", Damage);

            List<ConditionEffectDesc> effects = new List<ConditionEffectDesc>();
            foreach (XElement k in e.Elements("ConditionEffect"))
                effects.Add(new ConditionEffectDesc(k));
            Effects = effects.ToArray();

            MultiHit = e.HasElement("MultiHit");
            PassesCover = e.HasElement("PassesCover");
            ArmorPiercing = e.HasElement("ArmorPiercing");
            Wavy = e.HasElement("Wavy");
            Parametric = e.HasElement("Parametric");
            Boomerang = e.HasElement("Boomerang");

            Amplitude = e.GetValue<float>("Amplitude", 0);
            Frequency = e.GetValue<float>("Frequency", 1);
            Magnitude = e.GetValue<float>("Magnitude", 3);
        }
    }

    public class ConditionEffectDesc
    {
        public readonly ConditionEffectIndex Effect;
        public readonly int DurationMS;

        public ConditionEffectDesc(ConditionEffectIndex effect, int durationMs)
        {
            Effect = effect;
            DurationMS = durationMs;
        }

        public ConditionEffectDesc(XElement e)
        {
            Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), e.Value.Replace(" ", ""));
            DurationMS = (int)(e.GetAttribute<float>("duration") * 1000);
        }
    }

    public class QuestDesc
    {
        public readonly int Level;
        public readonly int Priority;

        public QuestDesc(int level, int priority)
        {
            Level = level;
            Priority = priority;
        }
    }

    public class ContainerDesc : ObjectDesc
    {
        public readonly int[] SlotTypes;
        public readonly int[] Equipment;

        public ContainerDesc(XElement e, string id, ushort type)
            : base(e, id, type)
        {
            SlotTypes = e.GetValue<string>("SlotTypes")?.CommaToArray<int>();
            Equipment = e.GetValue<string>("Equipment")?.CommaToArray<int>();
        }
    }
}
