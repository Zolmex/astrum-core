using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities.Behaviors
{
    public interface IBehaviorTransition : IStateChild
    {
        public void Enter(Character host, BehaviorController controller);
        public string Tick(Character host, RealmTime time, BehaviorController controller);
    }
}
