using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class ProjectileState
    {
        private static readonly ProjectileFactory _factory = new ProjectileFactory(256); // Size will increase as new projectiles are created

        public byte BulletId { get; set; }
        public int TimeAlive { get; set; }
        public int TTL { get; private set; } // When TimeAlive reaches time to live, projectile dies
        public int Damage { get; private set; }
        public bool Dead { get; private set; }
        public bool MultiHit { get; private set; }

        private readonly HashSet<Entity> _hits = new HashSet<Entity>();

        public void Init(byte bulletId, ProjectileDesc projDesc)
        {
            Reset();

            BulletId = bulletId;

            MultiHit = projDesc.MultiHit;
        }

        public void SetDamage(int dmg)
        {
            Damage = dmg;
        }

        public void SetTTL(int ttl)
        {
            TimeAlive = 0;
            TTL = ttl;
        }

        public void HitAdd(Entity en)
        {
            _hits.Add(en);

            if (!MultiHit)
                Death();
        }

        public void Death()
        {
            Dead = true;
        }

        public void Reset()
        {
            Dead = false;
            TimeAlive = 0;
            TTL = 0;
            Damage = 0;
            _hits.Clear();
        }

        public void Recycle()
        {
            _factory.Push(this);
        }

        public static ProjectileState GetProjectile()
        {
            ProjectileState ret = _factory.Pop();
            if (ret == null)
                return new ProjectileState();
            return ret;
        }
    }
}
