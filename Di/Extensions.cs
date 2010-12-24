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

        public delegate void Action<T>(T arg);

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
    }
}
