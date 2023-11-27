using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class ProjectileState : IIdentifiable
    {
        private static int _projectileId;
        private static readonly ProjectileFactory _factory = new ProjectileFactory(500000);

        public int Id { get; set; }

        public int TimeAlive { get; set; }
        public int TTL { get; private set; } // When TimeAlive reaches time to live, projectile dies

        public ProjectileState()
        {
            Id = Interlocked.Increment(ref _projectileId);
        }

        public void SetTTL(int ttl)
        {
            TTL = ttl;
        }

        public void Recycle()
        {
            TimeAlive = 0;
            TTL = 0;

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
