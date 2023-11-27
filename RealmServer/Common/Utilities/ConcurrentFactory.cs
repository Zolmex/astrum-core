using System;
using System.Collections.Concurrent;

namespace Common.Utilities
{
    public class ConcurrentFactory<T>
    {
        private readonly ConcurrentQueue<T> _pool;
        private int _count;

        public ConcurrentFactory(int size, params object[] args)
        {
            _count = size;
            _pool = new ConcurrentQueue<T>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue((T)Activator.CreateInstance(typeof(T), args));
        }

        public T Pop()
        {
            if (_count == 0 || !_pool.TryDequeue(out T ret))
                return default;

            _count--;
            return ret;
        }

        public void Push(T obj)
        {
            _count++;
            _pool.Enqueue(obj);
        }
    }
}
