using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Common.Utilities
{
    public class ConcurrentFactory<T>
    {
        private readonly ConcurrentQueue<T> _pool;
        private int _count;

        public ConcurrentFactory(int size, params object[] args)
        {
            Interlocked.Exchange(ref _count, size);
            _pool = new ConcurrentQueue<T>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue((T)Activator.CreateInstance(typeof(T), args));
        }

        public T Pop()
        {
            if (_count == 0 || !_pool.TryDequeue(out T ret))
                return default;

            Interlocked.Decrement(ref _count);
            return ret;
        }

        public void Push(T obj)
        {
            Interlocked.Increment(ref _count);
            _pool.Enqueue(obj);
        }
    }
}
