using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class ProjectileFactory
    {
        private readonly ConcurrentQueue<ProjectileState> _pool = new ConcurrentQueue<ProjectileState>();
        private int _count;

        public ProjectileFactory(int size)
        {
            _count = size;
            for (int i = 0; i < size; i++)
                _pool.Enqueue(new ProjectileState());
        }

        public ProjectileState Pop()
        {
            if (_count == 0 || !_pool.TryDequeue(out ProjectileState ret))
                return null;

            _count--;
            return ret;
        }

        public void Push(ProjectileState obj)
        {
            _count--;
            _pool.Enqueue(obj);
        }
    }
}
