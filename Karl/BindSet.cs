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
namespace Karl
{
    public class BindSet<T> : ISet<T>
    {
        public Event1<T> Added = new Event1<T>();
        public Event1<T> Removed = new Event1<T>();
        public Event0 Cleared = new Event0();
        public Event0 Changed = new Event0();

        private readonly ISet<T> elems;

        public BindSet()
        {
            Added.Add(t => { Changed.Handler(); });
            Removed.Add(t => { Changed.Handler(); });
            Cleared.Add(() => { Changed.Handler(); });
            elems = new HashSet<T>();
        }

        public void Add(T item)
        {
            ((ISet<T>) this).Add(item);
        }

        bool ISet<T>.Add(T item)
        {
            bool result = elems.Add(item);
            if (result)
            {
                Added.Handler(item);
            }
            return result;
        }

        public void Clear()
        {
            elems.Clear();
            Cleared.Handler();
        }

        public bool Contains(T item)
        {
            return elems.Contains(item);
        }

        public int Count
        {
            get { return elems.Count; }
        }
		
		public bool IsReadOnly
		{
			get { return elems.IsReadOnly; }
		}

        public IEnumerator<T> GetEnumerator()
        {
            return elems.GetEnumerator();
        }
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return elems.GetEnumerator();
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			elems.CopyTo(array, arrayIndex);
		}

        public bool Remove(T item)
        {
            bool result = elems.Remove(item);
            if (result)
            {
                Removed.Handler(item);
            }
            return result;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
    }
}

