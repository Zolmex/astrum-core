using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class Character : Entity
    {
        protected RealmTime _lastTick;

        public Character(ushort objType) : base(objType)
        {

        }

        public virtual bool Tick(RealmTime time)
        {
            if (Dead || (_lastTick.TickCount != 0 && _lastTick.TickCount != time.TickCount - 1)) // Ensure that ticks only occur once per logic tick
                return false;
            _lastTick = time;

            return true;
        }
    }
}
