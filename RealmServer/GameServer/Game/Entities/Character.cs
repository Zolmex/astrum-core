using Common;
using Common.Resources.Xml;
using GameServer.Game.Collections;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class Character : Entity
    {
        protected RealmTime _lastTick;

        protected readonly Random _rand = new Random();
        protected readonly ProjectileCollection _projectiles = new ProjectileCollection();

        public Character(ushort objType) : base(objType)
        {

        }

        public virtual bool Tick(RealmTime time)
        {
            if (Dead || (_lastTick.TickCount != 0 && _lastTick.TickCount != time.TickCount - 1)) // Ensure that ticks only occur once per logic tick
                return false;

            _lastTick = time;

            _projectiles.Update();

            return true;
        }

        public void ShootProjectile(ProjectileDesc projectile, byte numShots, float angle, WorldPosData startPos, float angleInc)
        {
            for (var i = 0; i < numShots; i++)
            {
                var projState = ProjectileState.GetProjectile();
                projState.SetTTL(projectile.LifetimeMS);
                _projectiles.Add(projState);

                var dmg = (uint)_rand.Next(projectile.MinDamage, projectile.MaxDamage);
                World.BroadcastAll(plr =>
                {
                    if (plr.DistSqr(this) >= Player.SIGHT_RADIUS_SQR)
                    {
                        plr.User.SendPacket(PacketId.ENEMYSHOOT, EnemyShoot.Write(plr.User,
                            projState.Id,
                            Id,
                            projectile.BulletId,
                            startPos,
                            angle,
                            dmg,
                            numShots,
                            angleInc
                            ));
                    }
                });
            }
        }
    }
}
