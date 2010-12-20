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
namespace Di
{
    public class BindList<T> : IList<T>
    {
        public delegate void ItemAdded(BindList<T> list, int index, T item);
        public delegate void ItemRemoved(BindList<T> list, int index, T item);
        public delegate void ListCleared(BindList<T> list);
		public delegate void ListChanged(BindList<T> list);
		
		private ListChanged changed;
        private ItemAdded added;
        private ItemRemoved removed;
        private ListCleared cleared;

        public class Events
        {
            BindList<T> list;

            public event ItemAdded Added
            {
                add { list.added += value; }
                remove { list.added -= value; }
            }

            public event ItemRemoved Removed
            {
                add { list.removed += value; }
                remove { list.removed -= value; }
            }
    
            public event ListCleared Cleared
            {
                add { list.cleared += value; }
                remove { list.cleared -= value; }
            }
			
			public event ListChanged Changed
			{
				add { list.changed += value; }
				remove { list.changed -= value; }
			}

            public Events(BindList<T> _list)
            {
                list = _list;
            }
        }

        public readonly Events Event;

        private readonly IList<T> list;

        public BindList()
        {
			changed = (l) => { return; };
			added = (l, i, t) => { changed(this); };
			removed = (l, i, t) => { changed(this); };
			cleared = (l) => { changed(this); };
            Event = new Events(this);
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
            added(this, list.Count - 1, item);
        }

        public void Clear()
        {
            list.Clear();
            cleared(this);
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
            get
            {
                return list.Count;
            }
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
            added(this, index, item);
        }

        public bool IsReadOnly
        {
            get
            {
                return list.IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                T old = list[index];
                list[index] = value;
                removed(this, index, old);
                added(this, index, value);
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            bool result = list.Remove(item);
            if (result)
            {
                removed(this, index, item);
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            removed(this, index, item);
        }
    }
}

