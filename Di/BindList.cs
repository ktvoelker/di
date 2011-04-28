//  
//  BindList.cs
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
    public class BindList<T> : IList<T>
    {
        public Event2<int, T> Added = new Event2<int, T>();
        public Event2<int, T> Removed = new Event2<int, T>();
        public Event0 Cleared = new Event0();
        public Event0 Changed = new Event0();

        private readonly IList<T> list;

        public BindList()
        {
            Added.Add((i, t) => { Changed.Handler(); });
            Removed.Add((i, t) => { Changed.Handler(); });
            Cleared.Add(() => { Changed.Handler(); });
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
            Added.Handler(list.Count - 1, item);
        }

        public void Clear()
        {
            list.Clear();
            Cleared.Handler();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            Added.Handler(index, item);
        }

        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                T old = list[index];
                list[index] = value;
                Removed.Handler(index, old);
                Added.Handler(index, value);
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            bool result = list.Remove(item);
            if (result)
            {
                Removed.Handler(index, item);
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            Removed.Handler(index, item);
        }
    }
}

