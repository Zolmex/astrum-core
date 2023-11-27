using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class PlayerInventory : EntityInventory
    {
        private const int MAX_INV_SLOTS = 20;

        private int _invSize;

        public PlayerInventory(Player player) : base(player, MAX_INV_SLOTS)
        {
        }

        public void Load(DbChar chr)
        {
            if (chr.Inventory == null)
                return;

            _invSize = chr.HasBackpack ? 20 : 12;
            SetItems(chr.Inventory);
        }

        public void Save(DbChar chr)
        {
            if (chr.Inventory == null)
                return;

            chr.Inventory = _itemTypes;
        }
    }
}
