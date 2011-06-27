using System;
using System.Collections.Generic;
using System.Threading;

namespace Karl
{
    public static class EqCache<K, T> where T : class
    {
        private static IDictionary<K, WeakReference<T>> cache = new Dictionary<K, WeakReference<T>>();

        private static object cacheLock = new object();

        internal static int Hits = 0;

        internal static int Misses = 0;

        public static U Get<U>(K key, Func<K, U> maker) where U : class, T
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
    }
}
