using Common;
using Common.Resources.Xml;
using GameServer.Game.Collections;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Incoming;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private const float MIN_ATTACK_MULT = 0.5f;
        private const float MAX_ATTACK_MULT = 2f;

        public bool ShotsVisible(Player player)
        {
            switch (User.GameInfo.AllyShots)
            {
                case 0:
                    return true; // All
                case 1:
                    return false; // None
                case 2:
                    return false; // Locked
                case 3:
                    return false; // Guild
                case 4:
                    return false; // Both
                default:
                    return true;
            }
        }

        public void Shoot(ProjectileDesc projDesc, byte numShots, float angle)
        {
            for (var i = 0; i < numShots; i++)
            {
                var projIndex = _lastProjectileId++;
                var proj = Projectiles[projIndex];
                if (proj == null)
                {
                    proj = ProjectileState.GetProjectile();
                    Projectiles[projIndex] = proj;
                }

                proj.Init(projIndex, projDesc);
                proj.SetTTL(projDesc.LifetimeMS);

                var dmg = User.Random.NextIntRange((uint)projDesc.MinDamage, (uint)projDesc.MaxDamage) * GetAttackMultiplier();
                proj.SetDamage((int)dmg);

                World.BroadcastAll(plr =>
                {
                    if (plr.ShotsVisible(this) && plr.DistSqr(this) <= SIGHT_RADIUS_SQR && plr.Id != Id)
                    {
                        AllyShoot.Write(plr.User.Network, Id, projDesc.ContainerType, angle);
                    }
                });
            }
        }

        private float GetAttackMultiplier()
        {
            //if (isWeak()) // Condition effects not added yet
            //{
            //    return MIN_ATTACK_MULT;
            //}
            var attMult = MIN_ATTACK_MULT + Stats.Get<int>(StatType.Attack) / 75f * (MAX_ATTACK_MULT - MIN_ATTACK_MULT);
            //if (isDamaging())
            //{
            //    attMult = attMult * 1.5;
            //}
            return attMult;
        }
    }
}
