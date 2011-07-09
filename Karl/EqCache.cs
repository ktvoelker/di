using System;
using System.Collections.Generic;
using System.Threading;

namespace Karl
{
    public interface IEqCache<K, T> where T : class
    {
        U Get<U>(K key, Func<K, U> maker) where U : class, T;
    }

    public class EqCache<K, T> : IEqCache<K, T> where T : class
    {
        private IDictionary<K, WeakReference<T>> cache = new Dictionary<K, WeakReference<T>>();

        private object cacheLock = new object();

        internal int Hits = 0;

        internal int Misses = 0;

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
                    cache[key] = new WeakReference<T>(result);
                }
            }
            return result;
        }

        public class AbstractView<L, U> : IEqCache<L, U>
            where L : K
            where U : class, T
        {
            private readonly EqCache<K, T> parent;

            public AbstractView(EqCache<K, T> _parent)
            {
                parent = _parent;
            }

            public new V Get<V>(K key, Func<K, V> maker) where V : class, U
            {
                return parent.Get<V>(key, maker);
            }
        }

        public class ConcreteView<L, U> : AbstractView<L, U> where U : class
        {
            private readonly Func<K, U> maker;

            public ConcreteView(EqCache<K, T> _parent, Func<K, U> _maker) : base(_parent)
            {
                maker = _maker;
            }

            public U Get(K key)
            {
                return Get<U>(key, maker);
            }
        }
    }
}
