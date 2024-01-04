using Common;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        public static readonly BehaviorState Pirate =
            new BehaviorState(
                new BehaviorState("0",
                    new Follow(0.85f, range: 1, coolDownMS: 5000),
                    new Wander(0.4f),
                    new Shoot(3, coolDownMS: 2500)
                    )
                );
    }
}
