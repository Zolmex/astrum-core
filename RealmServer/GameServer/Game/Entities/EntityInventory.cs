using Common;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class EntityInventory
    {
        protected readonly Logger _log;

        protected int[] _itemTypes;

        protected readonly Entity _entity;
        protected readonly object _invLock = new object();

        public int this[int slot]
        {
            get => GetItem(slot);
            set => SetItem(slot, value);
        }

        public EntityInventory(Entity entity, int numSlots)
        {
            _log = new Logger(GetType());

            _entity = entity;
            _itemTypes = new int[numSlots];
        }

        protected void SetItem(int slot, int itemType)
        {
            lock (_invLock)
            {
                if (slot < 0 || slot >= _itemTypes.Length)
                {
                    _log.Warn($"Inventory transaction failed for slot {slot} item {itemType}");
                    return;
                }

                _itemTypes[slot] = itemType;

                _entity.Stats.Set(GetStat(slot), itemType);
            }
        }

        protected void SetSlots(int[] itemTypes, params int[] slots)
        {
            lock (_invLock)
            {
                if (slots == null || itemTypes == null || slots.Length == 0 || itemTypes.Length == 0 || slots.Length != itemTypes.Length)
                {
                    _log.Warn($"Inventory transaction failed for slots {slots?.ToCommaSepString()} item {itemTypes?.ToCommaSepString()}");
                    return;
                }

                var i = 0;
                foreach (var slot in slots)
                {
                    var itemType = itemTypes[i];
                    if (slot < 0 || slot >= _itemTypes.Length)
                    {
                        i++;
                        _log.Warn($"Inventory transaction failed for slot {slot} item {itemType}");
                        continue;
                    }

                    i++;
                    _itemTypes[slot] = itemType;

                    _entity.Stats.Set(GetStat(slot), itemType);
                }
            }
        }

        protected void SetItems(int[] items)
        {
            lock (_invLock)
            {
                if (items == null || items.Length > _itemTypes.Length)
                {
                    _log.Warn($"Inventory transaction failed. Items Size: {items.Length} Inv Size: {_itemTypes.Length}");
                    return;
                }

                for (var i = 0; i < items.Length; i++)
                {
                    _itemTypes[i] = items[i];
                    _entity.Stats.Set(GetStat(i), _itemTypes[i]);
                }
            }
        }

        protected int GetItem(int slot)
        {
            lock (_invLock)
            {
                if (slot < 0 || slot > _itemTypes.Length)
                    return 0;

                return _itemTypes[slot];
            }
        }

        protected StatType GetStat(int slot)
        {
            switch (slot)
            {
                case 0: return StatType.Inventory0;
                case 1: return StatType.Inventory1;
                case 2: return StatType.Inventory2;
                case 3: return StatType.Inventory3;
                case 4: return StatType.Inventory4;
                case 5: return StatType.Inventory5;
                case 6: return StatType.Inventory6;
                case 7: return StatType.Inventory7;
                case 8: return StatType.Inventory8;
                case 9: return StatType.Inventory9;
                case 10: return StatType.Inventory10;
                case 11: return StatType.Inventory11;
                case 12: return StatType.Backpack0;
                case 13: return StatType.Backpack1;
                case 14: return StatType.Backpack2;
                case 15: return StatType.Backpack3;
                case 16: return StatType.Backpack4;
                case 17: return StatType.Backpack5;
                case 18: return StatType.Backpack6;
                case 19: return StatType.Backpack7;
                default:
                    throw new ArgumentException($"Invalid inventory slot {slot}");
            }
        }
    }
}
