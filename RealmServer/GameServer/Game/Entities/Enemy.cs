using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameServer.Game.Entities.Player;

namespace GameServer.Game.Entities
{
    public class Enemy : Character
    {
        public Enemy(ushort objType) : base(objType)
        {

        }
    }
}
