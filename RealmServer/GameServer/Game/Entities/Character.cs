using Common;
using Common.Resources.Xml;
using GameServer.Game.Collections;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static GameServer.Game.Entities.Player;

namespace GameServer.Game.Entities
{
    public class Character : Entity
    {
        public readonly ProjectileState[] Projectiles = new ProjectileState[256];

        protected readonly Random _rand = new Random();
        protected byte _lastProjectileId;
        protected RealmTime _lastTick;

        public Character(ushort objType) : base(objType)
        {

        }

        public virtual bool Tick(RealmTime time)
        {
            if (!_initialized || Dead || (_lastTick.TickCount != 0 && _lastTick.TickCount != time.TickCount - 1)) // Ensure that ticks only occur once per logic tick
                return false;

            var timeDiff = (int)(time.TotalElapsedMs - _lastTick.TotalElapsedMs);
            foreach (var proj in Projectiles)
            {
                if (proj == null)
                    continue;

                proj.TimeAlive += timeDiff;

                if (proj.TimeAlive >= proj.TTL)
                    proj.Death();
            }

            _lastTick = time;


            return true;
        }

        public void ShootProjectile(ProjectileDesc projDesc, byte numShots, float angle, WorldPosData startPos, float angleInc)
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

                var dmg = (uint)_rand.Next(projDesc.MinDamage, projDesc.MaxDamage);
                proj.SetDamage((int)dmg);

                World.BroadcastAll(plr =>
                {
                    if (plr.DistSqr(this) >= Player.SIGHT_RADIUS_SQR)
                    {
                        EnemyShoot.Write(plr.User.Network,
                            proj.BulletId,
                            Id,
                            projDesc.BulletId,
                            startPos,
                            angle,
                            dmg,
                            numShots,
                            angleInc
                            );
                    }
                });
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            for (var i = 0; i < Projectiles.Length; i++)
            {
                var proj = Projectiles[i];
                if (proj == null)
                    continue;

                proj.Death();
                proj.Recycle();

                Projectiles[i] = null;
            }
        }
    }
}
