using Common.Utilities.Net;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public struct IntPoint
    {
        public int X;
        public int Y;

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(IntPoint a, IntPoint b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(IntPoint a, IntPoint b) => a.X != b.X || a.Y != b.Y;

        public bool Equals(IntPoint other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj)
        {
            if (obj is IntPoint p)
            {
                return Equals(p);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Y << 16) ^ X;
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}";
        }
    }

    public struct ObjectStatusData
    {
        public int ObjectId;
        public WorldPosData Pos;
        public List<StatData> Stats;

        public static ObjectStatusData Read(NetworkReader rdr)
        {
            var ret = new ObjectStatusData();
            ret.ObjectId = rdr.ReadInt32();
            ret.Pos = WorldPosData.Read(rdr);
            ret.Stats = new List<StatData>(rdr.ReadByte());
            for (var i = 0; i < ret.Stats.Capacity; i++)
                ret.Stats.Add(StatData.Read(rdr));
            return ret;
        }

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(ObjectId);
            Pos.Write(wtr);
            wtr.Write((byte)Stats.Count);
            for (var i = 0; i < Stats.Count; i++)
                Stats[i].Write(wtr);
        }

        public void Update(WorldPosData pos, StatType type, object value)
        {
            Pos = pos;

            var stat = new StatData();
            stat.Type = type;
            stat.SetValue(value);

            Stats.Add(stat);
        }
    }

    public struct WorldPosData
    {
        public float X;
        public float Y;

        public static WorldPosData Read(NetworkReader rdr)
            => new WorldPosData()
            {
                X = rdr.ReadSingle(),
                Y = rdr.ReadSingle()
            };

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(X);
            wtr.Write(Y);
        }
    }

    public struct StatData
    {
        public StatType Type;
        public int Value;
        public string Text;

        public StatData(StatType type, object value)
        {
            Type = type;
            SetValue(value);
        }

        public void SetValue(object value)
        {
            if (IsStringStat(Type))
                Text = (string)value;
            else
                Value = (int)value;
        }

        public static bool IsStringStat(StatType type)
        {
            switch (type)
            {
                case StatType.Name:
                case StatType.GuildName:
                case StatType.InventoryData0:
                case StatType.InventoryData1:
                case StatType.InventoryData2:
                case StatType.InventoryData3:
                case StatType.InventoryData4:
                case StatType.InventoryData5:
                case StatType.InventoryData6:
                case StatType.InventoryData7:
                case StatType.InventoryData8:
                case StatType.InventoryData9:
                case StatType.InventoryData10:
                case StatType.InventoryData11:
                case StatType.InventoryData12:
                case StatType.InventoryData13:
                case StatType.InventoryData14:
                case StatType.InventoryData15:
                case StatType.InventoryData16:
                case StatType.InventoryData17:
                case StatType.InventoryData18:
                case StatType.InventoryData19:
                    return true;
                default:
                    return false;
            }
        }

        public static StatData Read(NetworkReader rdr)
        {
            var ret = new StatData();
            ret.Type = (StatType)rdr.ReadByte();
            if (IsStringStat(ret.Type))
                ret.Text = rdr.ReadUTF();
            else ret.Value = rdr.ReadInt32();
            return ret;
        }

        public void Write(NetworkWriter wtr)
        {
            wtr.Write((byte)Type);
            if (IsStringStat(Type))
                wtr.WriteUTF(Text);
            else wtr.Write(Value);
        }
    }

    public struct TileData
    {
        public short X;
        public short Y;
        public ushort GroundType;

        public static TileData Read(NetworkReader rdr)
            => new TileData()
            {
                X = rdr.ReadInt16(),
                Y = rdr.ReadInt16(),
                GroundType = rdr.ReadUInt16()
            };

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(X);
            wtr.Write(Y);
            wtr.Write(GroundType);
        }
    }

    public struct ObjectData
    {
        public ushort ObjectType;
        public ObjectStatusData Status;

        public static ObjectData Read(NetworkReader rdr)
            => new ObjectData()
            {
                ObjectType = rdr.ReadUInt16(),
                Status = ObjectStatusData.Read(rdr)
            };

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(ObjectType);
            Status.Write(wtr);
        }
    }

    public struct ObjectDropData
    {
        public int ObjectId;
        public bool Explode;

        public static ObjectDropData Read(NetworkReader rdr)
            => new ObjectDropData()
            {
                ObjectId = rdr.ReadInt32(),
                Explode = rdr.ReadBoolean()
            };

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(ObjectId);
            wtr.Write(Explode);
        }
    }

    public struct SlotObjectData
    {
        public int ObjectId;
        public byte SlotId;

        public static SlotObjectData Read(NetworkReader rdr)
            => new SlotObjectData()
            {
                ObjectId = rdr.ReadInt32(),
                SlotId = rdr.ReadByte()
            };

        public void Write(NetworkWriter wtr)
        {
            wtr.Write(ObjectId);
            wtr.Write(SlotId);
        }
    }
}
