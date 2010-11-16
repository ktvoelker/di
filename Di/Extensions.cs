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
    }
}

