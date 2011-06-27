//  
//  Extensions.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2010 Karl Voelker
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Karl
{
    public static class Extensions
    {
        public static IEnumerable<T> Force<T>(this IEnumerable<T> elems)
        {
            return elems.ToList();
        }

        public static IEnumerable<int> Indices<T>(this IEnumerable<T> elems)
        {
			int c = elems.Count();
			for (int i = 0; i < c; ++i)
			{
				yield return i;
			}
        }

        public static int Count<T>(this IEnumerable<T> elems)
        {
            int count = 0;
            var e = elems.GetEnumerator();
            while (e.MoveNext())
            {
                ++count;
            }
            return count;
        }

        public static T Item<T>(this IEnumerable<T> elems, int index)
        {
            foreach (T elem in elems)
            {
                if (index == 0)
                {
                    return elem;
                }
                --index;
            }
            throw new IndexOutOfRangeException();
        }

        public static void ForEach<T>(this IEnumerable<T> elems, Action<T> a)
        {
            foreach (T elem in elems)
            {
                a(elem);
            }
        }

        public static IEnumerable<Tuple<T, T>> WithPrevCircular<T>(this IEnumerable<T> elems)
        {
            var list = elems.ToList();
            if (list.Count == 0)
            {
                yield break;
            }
            yield return new Tuple<T, T>(list.Last(), list.First());
            for (int i = 1; i < list.Count; ++i)
            {
                yield return new Tuple<T, T>(list[i - 1], list[i]);
            }
        }

        public static IEnumerable<Tuple<T, T>> WithNextCircular<T>(this IEnumerable<T> elems)
        {
            var list = elems.ToList();
            if (list.Count == 0)
            {
                yield break;
            }
            for (int i = 0; i < list.Count - 1; ++i)
            {
                yield return new Tuple<T, T>(list[i], list[i + 1]);
            }
            yield return new Tuple<T, T>(list.Last(), list.First());
        }

        public static U FoldLeft<T, U>(this IEnumerable<T> elems, U zero, Func<U, T, U> func)
        {
            U result = zero;
            foreach (T elem in elems)
            {
                result = func(result, elem);
            }
            return result;
        }

        public static T FoldLeft1<T>(this IEnumerable<T> elems, Func<T, T, T> func)
        {
            T result = elems.First();
            foreach (T elem in elems.Skip(1))
            {
                result = func(result, elem);
            }
            return result;
        }

        public static U FoldRight<T, U>(this IEnumerable<T> elems, U zero, Func<T, U, U> func)
        {
            U result = zero;
            foreach (T elem in elems.Reverse())
            {
                result = func(elem, result);
            }
            return result;
        }

        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> elems)
        {
            var list = new List<T>(elems);
            for (int i = list.Count - 1; i >= 0; --i)
            {
                yield return list[i];
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> xss)
        {
            foreach (var xs in xss)
            {
                foreach (var x in xs)
                {
                    yield return x;
                }
            }
        }

        public static bool HasAny<T>(this IEnumerable<T> elems)
        {
            return elems.GetEnumerator().MoveNext();
        }

        public static Func<U> Apply<T, U>(this Func<T, U> f, T t)
        {
            return () => f(t);
        }

        public static Func<U, V> Apply<T, U, V>(this Func<T, U, V> f, T t)
        {
            return u => f(t, u);
        }

        public static Func<U, V, W> Apply<T, U, V, W>(this Func<T, U, V, W> f, T t)
        {
            return (u, v) => f(t, u, v);
        }

        public static Action Apply<T>(this Action<T> f, T t)
        {
            return () => f(t);
        }

        public static Action<U> Apply<T, U>(this Action<T, U> f, T t)
        {
            return (u) => f(t, u);
        }

        public static Action Apply<T, U>(this Action<T, U> f, T t, U u)
        {
            return () => f(t, u);
        }

        public static Action<U, V> Apply<T, U, V>(this Action<T, U, V> f, T t)
        {
            return (u, v) => f(t, u, v);
        }

        public static V GetWithDefault<K, V>(this IDictionary<K, V> dict, K key, V fallback)
        {
            V result;
            return dict.TryGetValue(key, out result) ? result : fallback;
        }

        private static Regex TrueString = new Regex("^(y(es)?|t(rue)?|1|enable(d)?|on)$", RegexOptions.IgnoreCase);

        private static Regex FalseString = new Regex("^(n(o)?|f(alse)?|0|disable(d)?|off)$", RegexOptions.IgnoreCase);

        public static bool ToBool(this string xs)
        {
            if (TrueString.IsMatch(xs))
            {
                return true;
            }
            else if (FalseString.IsMatch(xs))
            {
                return false;
            }
            else
            {
                throw new FormatException("`" + xs + "' has no Boolean interpretation.");
            }
        }

        public static bool GetBoolWithDefault<K>(this IDictionary<K, string> dict, K key, bool fallback)
        {
            string xs;
            if (dict.TryGetValue(key, out xs))
            {
                return xs.ToBool();
            }
            else
            {
                return fallback;
            }
        }

        public static IEnumerable<BindListPointer<T>> Pointers<T>(this BindList<T> list) where T : class
        {
            foreach (var i in list.Indices())
            {
                yield return new BindListPointer<T>(list, i);
            }
        }

        public static IEnumerable<T> AsSingleton<T>(this T elem)
        {
            yield return elem;
        }
    }
}
