using System;
using System.Collections.Generic;

namespace Common.Utilities
{
    public class Factory<T>
    {
        private readonly Queue<T> _pool;

        public Factory(int size, params object[] args)
        {
            _pool = new Queue<T>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue((T)Activator.CreateInstance(typeof(T), args));
        }

        public T Pop()
        {
            if (_pool.Count < 1)
                throw new Exception("Object pool is empty.");

            if (!_pool.TryDequeue(out T ret))
                throw new Exception("Failed to remove object from the beginning of the queue.");

            return ret;
        }

        public void Push(T obj)
            => _pool.Enqueue(obj);
    }

    public class Factory
    {
        private readonly Queue<object> _pool;

        public Factory(Type type, int size, params object[] args)
        {
            _pool = new Queue<object>();
            for (int i = 0; i < size; i++)
                _pool.Enqueue(Activator.CreateInstance(type, args));
        }

        public object Pop()
        {
            if (_pool.Count < 1)
                throw new Exception("Object pool is empty.");

            if (!_pool.TryDequeue(out object ret))
                throw new Exception("Failed to remove object from the beginning of the queue.");

            return ret;
        }

        public void Push(object obj)
            => _pool.Enqueue(obj);
    }
}
