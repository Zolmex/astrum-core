using Common.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class SmartCollection<T> : IEnumerable<KeyValuePair<int, T>> where T : IIdentifiable
    {
        public int Count { get; protected set; }

        protected readonly Dictionary<int, T> _dict = new Dictionary<int, T>();
        protected readonly ConcurrentQueue<T> _adds = new ConcurrentQueue<T>();
        protected readonly ConcurrentQueue<int> _drops = new ConcurrentQueue<int>();

        public virtual void Update()
        {
            while (_adds.TryDequeue(out var newValue))
                lock (_dict)
                    if (_dict.TryAdd(newValue.Id, newValue))
                        Count++;

            while (_drops.TryDequeue(out var oldValueId))
                lock (_dict)
                    if (_dict.Remove(oldValueId))
                        Count--;
        }

        public T Get(int itemId)
        {
            lock (_dict)
            {
                if (!_dict.TryGetValue(itemId, out var ret))
                    return default;
                return ret;
            }
        }

        public virtual void Add(T item)
        {
            _adds.Enqueue(item);
        }

        public virtual void Remove(int itemId)
        {
            _drops.Enqueue(itemId);
        }

        public virtual IEnumerable<T> GetAll()
            => _dict.Values;

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
        }

        public void Clear()
        {
            lock (_dict)
                _dict.Clear();
            _adds.Clear();
            _drops.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
        }
    }
}
