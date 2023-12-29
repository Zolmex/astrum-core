using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities.Behaviors
{
    public interface IBehaviorAction : IStateChild
    {
        public void Enter(Character host, BehaviorController controller);
        public void Tick(Character host, RealmTime time, BehaviorController controller);
        public void Exit(Character host, RealmTime time, BehaviorController controller);
    }
}
