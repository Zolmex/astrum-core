using Common;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class ShootInfo
    {
        public int CoolDownLeft;
    }

    public class Shoot : IBehaviorAction
    {
        private readonly float _radiusSqr;
        private readonly byte _count;
        private readonly float _shootAngle;
        private readonly byte _projIndex;
        private readonly float _fixedAngle;
        private readonly float _angleOffset;
        private readonly float _predictive;
        private readonly int _coolDownOffset;
        private readonly int _coolDownMS;

        public Shoot(XElement xml)
        {
            _radiusSqr = xml.GetAttribute<float>("radius");
            _radiusSqr *= _radiusSqr;
            _count = xml.GetAttribute<byte>("count", 1);
            _shootAngle = xml.GetAttribute<float>("shootAngle") * (float)(Math.PI / 180);
            _projIndex = xml.GetAttribute<byte>("projIndex");
            _fixedAngle = xml.GetAttribute<float>("fixedAngle");
            _angleOffset = xml.GetAttribute<float>("angleOffset");
            _predictive = xml.GetAttribute<float>("predictive");
            _coolDownOffset = xml.GetAttribute<int>("coolDownOffset");
            _coolDownMS = xml.GetAttribute<int>("coolDownMS");
        }

        public Shoot(float radius, byte count = 1, float shootAngle = 0f,
            byte projIndex = 0, float fixedAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int coolDownMS = 0)
        {
            _radiusSqr = radius * radius;
            _count = count;
            _shootAngle = shootAngle * (float)(Math.PI / 180);
            _projIndex = projIndex;
            _fixedAngle = fixedAngle * (float)(Math.PI / 180);
            _angleOffset = angleOffset * (float)(Math.PI / 180);
            _predictive = predictive;
            _coolDownOffset = coolDownOffset;
            _coolDownMS = coolDownMS;
        }

        public void Enter(Character host, BehaviorController controller)
        {
            var shootInfo = new ShootInfo();
            shootInfo.CoolDownLeft = _coolDownOffset;

            controller.InsertResource(this, shootInfo);
        }

        public void Tick(Character host, RealmTime time, BehaviorController controller)
        {
            var shootInfo = (ShootInfo)controller.GetResource(this);

            if (shootInfo.CoolDownLeft > 0)
            {
                shootInfo.CoolDownLeft -= time.ElapsedMsDelta;
                return;
            }

            //if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
            //    return;

            var attackTarget = host.GetAttackTarget(_radiusSqr);
            if (attackTarget == null)
                return;

            shootInfo.CoolDownLeft = _coolDownMS;

            var startAngle = _fixedAngle == 0 ? (float)Math.Atan2(attackTarget.Position.Y - host.Position.Y, attackTarget.Position.X - host.Position.X) : _fixedAngle;
            startAngle += _angleOffset;
            // TODO: predictive code

            host.ShootProjectile(host.Desc.Projectiles[_projIndex], _count, startAngle, host.Position, _shootAngle);
        }

        public void Exit(Character host, RealmTime time, BehaviorController controller)
        {

        }
    }
}
