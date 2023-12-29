using Common;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class WanderInfo
    {
        public Vector2 Direction;
        public float DistLeft;
    }

    public class Wander : IBehaviorAction
    {
        private readonly float _speed;

        public Wander(XElement xml)
        {
            _speed = xml.GetAttribute<float>("speed");
        }

        public Wander(float speed)
        {
            _speed = speed;
        }

        public void Enter(Character host, BehaviorController controller)
        {
            controller.InsertResource(this, new WanderInfo());
        }

        public void Tick(Character host, RealmTime time, BehaviorController controller)
        {
            //if (_host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            //    return;

            var resource = (WanderInfo)controller.GetResource(this);
            var distLeft = resource.DistLeft;
            if (distLeft <= 0)
            {
                var rand = new Random();
                resource.Direction = new Vector2(rand.Next() % 2 == 0 ? -1 : 1, rand.Next() % 2 == 0 ? -1 : 1);
                Vector2.Normalize(resource.Direction);
                resource.DistLeft = 600 / 1000f;
            }
            var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            host.Move(host.Position.X + resource.Direction.X * dist, host.Position.Y + resource.Direction.Y * dist);
            resource.DistLeft -= dist;
        }

        public void Exit(Character host, RealmTime time, BehaviorController controller)
        {
            // Empty
        }
    }
}
