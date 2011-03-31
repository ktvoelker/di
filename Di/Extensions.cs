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
namespace Di
{
    public static class Extensions
    {
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

        public static CharIter Item(this Gtk.TextBuffer buffer, int index)
        {
            return new CharIter(buffer.GetIterAtOffset(index));
        }

        public static CharIter GetCursorIter(this Gtk.TextBuffer buffer)
        {
            return buffer.Item(buffer.CursorPosition);
        }

        public static void Delete(this Gtk.TextBuffer buffer, Range r)
        {
            var start = r.Start.GtkIter;
            var end = r.End.GtkIter;
            buffer.DeleteInteractive(ref start, ref end, true);
        }

        public static string GetName(this IEnumerable<Controller.WindowMode> mode)
        {
            return string.Join("-", mode.Where(m => !m.Hidden).OrderByDescending(m => m.KeyMap.Priority).Select(m => m.Name).ToArray());
        }
		
		public static Gdk.Size GetSize(this Pango.FontDescription font)
		{
			var widget = new Gtk.TextView();
			widget.ModifyFont(font);
			var layout = widget.CreatePangoLayout("W");
			int width, height;
			layout.GetPixelSize(out width, out height);
			return new Gdk.Size() { Width = width, Height = height };
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

        public static string ProjectRelativeFullName(this Model.IFsQueryable node)
        {
            if (node == node.Root.Root)
            {
                return ".";
            }
            else
            {
                return node.FullName.Substring(node.Root.Root.FullName.Length + 1);
            }
        }

        public static bool ContainsFocus(this Gtk.Widget w)
        {
            var v = w as View.IContainFocus;
            return (v == null ? w : v.FocusWidget).HasFocus;
        }

        public static void GiveFocus(this Gtk.Widget w)
        {
            var v = w as View.IContainFocus;
            (v == null ? w : v.FocusWidget).GrabFocus();
        }
    }
}
