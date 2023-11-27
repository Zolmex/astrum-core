using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class Container : Entity
    {
        public readonly EntityInventory Inventory;

        public Container(ushort objType, int numSlots) : base(objType)
        {
            Inventory = new EntityInventory(this, numSlots);
        }
    }
}
