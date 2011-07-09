using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Karl
{
    public interface IEqCache<K, T> where T : class
    {
        U Get<U>(K key, Func<K, U> maker) where U : class, T;
    }

    public abstract class AbstractEqCache<K, T, R> : IEqCache<K, T>
        where T : class
        where R : IReference<T>
    {
        protected internal IDictionary<K, R> cache = new Dictionary<K, R>();

        private object cacheLock = new object();

        internal int Hits = 0;

        internal int Misses = 0;

        private Func<T, R> refMaker;

        private Func<K, T> maker;

        public AbstractEqCache(Func<T, R> _refMaker, Func<K, T> _maker)
        {
            refMaker = _refMaker;
            maker = _maker;
        }

        public void Clear()
        {
            lock (cacheLock)
            {
                cache.Clear();
                Hits = 0;
                Misses = 0;
            }
        }

        public T Get(K key)
        {
            return Get<T>(key, maker);
        }

        public U Get<U>(K key, Func<K, U> maker) where U : class, T
        {
            U result = null;
            lock (cacheLock)
            {
                if (cache.ContainsKey(key))
                {
                    var thing = cache[key].Target;
                    if (thing != null)
                    {
                        result = thing as U;
                        if (result == null)
                        {
                            throw new ArgumentException(string.Format("Cache entry type mismatch: expected {0}; found {1}.", typeof(U), thing.GetType()));
                        }
                        ++Hits;
                    }
                }
                if (result == null)
                {
                    ++Misses;
                    result = maker(key);
                    cache[key] = refMaker(result);
                }
            }
            return result;
        }
    }

    public class WeakEqCache<K, T> : AbstractEqCache<K, T, WeakReference<T>> where T : class
    {
        public WeakEqCache(Func<K, T> maker)
            : base(obj => new WeakReference<T>(obj), maker)
        {
            // empty
        }
    }

    public class StrongEqCache<K, T> : AbstractEqCache<K, T, StrongReference<T>> where T : class
    {
        public StrongEqCache(Func<K, T> maker)
            : base(obj => new StrongReference<T>(obj), maker)
        {
            // empty
        }

        public IEnumerable<T> GetAll()
        {
            return cache.Values.Select(r => r.Target);
        }
    }
}
