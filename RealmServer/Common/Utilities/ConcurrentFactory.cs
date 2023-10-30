using System;
using System.Collections.Concurrent;

namespace Common.Utilities
{
    public class ConcurrentFactory<T>
    {
        private readonly ConcurrentQueue<T> _pool;

        public ConcurrentFactory(int size, params object[] args)
        {
            _pool = new ConcurrentQueue<T>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue((T)Activator.CreateInstance(typeof(T), args));
        }

        public T Pop()
        {
            if (_pool.IsEmpty)
                throw new Exception("Object pool is empty.");

            if (!_pool.TryDequeue(out T ret))
                throw new Exception("Failed to remove object from the beginning of the queue.");

            return ret;
        }

        public void Push(T obj)
            => _pool.Enqueue(obj);
    }

    public class ConcurrentFactory
    {
        private readonly ConcurrentQueue<object> _pool;

        public ConcurrentFactory(Type type, int size, params object[] args)
        {
            _pool = new ConcurrentQueue<object>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue(Activator.CreateInstance(type, args));
        }

        public object Pop()
        {
            if (_pool.IsEmpty)
                throw new Exception("Object pool is empty.");

            if (!_pool.TryDequeue(out object ret))
                throw new Exception("Failed to remove object from the beginning of the queue.");

            return ret;
        }

        public void Push(object obj)
            => _pool.Enqueue(obj);
    }
}
