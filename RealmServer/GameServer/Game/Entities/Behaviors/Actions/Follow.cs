using Common;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class FollowInfo
    {
        public Vector2 TargetPos;
        public int CoolDownLeft;
    }

    public class Follow : IBehaviorAction
    {
        private readonly float _speed;
        private readonly float _rangeSqr;
        private readonly float _acquireRadiusSqr;
        private readonly int _coolDownMS;

        public Follow(XElement xml)
        {
            _speed = xml.GetAttribute<float>("speed", 1f);
            _rangeSqr = xml.GetAttribute<float>("range", 2f);
            _rangeSqr *= _rangeSqr;
            _acquireRadiusSqr = xml.GetAttribute<float>("acquireRange", 10f);
            _acquireRadiusSqr *= _acquireRadiusSqr;
            _coolDownMS = xml.GetAttribute<int>("coolDownMS", 1000);
        }

        public Follow(float speed = 1f, float range = 2f, float acquireRange = 10f, int coolDownMS = 1000)
        {
            _speed = speed;
            _rangeSqr = range * range;
            _acquireRadiusSqr = acquireRange * acquireRange;
            _coolDownMS = coolDownMS;
        }

        public void Enter(Character host, BehaviorController controller)
        {
            controller.InsertResource(this, new FollowInfo());
        }

        public void Tick(Character host, RealmTime time, BehaviorController controller)
        {
            var resource = (FollowInfo)controller.GetResource(this);
            if (resource.CoolDownLeft <= 0)
            {
                var target = host.GetAttackTarget(_acquireRadiusSqr);
                if (target == null)
                    return;

                resource.TargetPos = new Vector2(target.Position.X, target.Position.Y);
                resource.CoolDownLeft = _coolDownMS;
            }
            else resource.CoolDownLeft -= time.ElapsedMsDelta;

            //if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            //    return;

            var targetPos = resource.TargetPos;
            if (targetPos != Vector2.Zero && host.DistSqr(targetPos.X, targetPos.Y) > _rangeSqr)
            {
                var dist = host.GetSpeed(_speed) * time.ElapsedMsDelta / 1000f;
                var direction = new Vector2(targetPos.X > host.Position.X ? 1 : -1, targetPos.Y > host.Position.Y ? 1 : -1);
                host.Move(host.Position.X + direction.X * dist, host.Position.Y + direction.Y * dist);
            }
        }

        public void Exit(Character host, RealmTime time, BehaviorController controller)
        {
            // Empty
        }
    }
}
