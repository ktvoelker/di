//  
//  BindListPointer.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2011 Karl Voelker
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
namespace Di
{
    public class BindListPointer<T> where T : class
    {
        BindList<T> list;
        int index;
        bool valid;

        public readonly Event0 Removed = new Event0();

        public T Value
        {
            get
            {
                return Validate(() => list[index]);
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public BindListPointer(BindList<T> list, T elem) : this(list, list.IndexOf(elem))
        {
        }

        public BindListPointer(BindList<T> _list, int _index)
        {
            if (_index < 0 || _index >= _list.Count)
            {
                throw new IndexOutOfRangeException();
            }
            list = _list;
            index = _index;
            valid = true;
            list.Added.Add((n, elem) =>
            {
                if (n <= index)
                {
                    ++index;
                }
            });
            list.Removed.Add((n, elem) =>
            {
                if (n < index)
                {
                    --index;
                }
                else if (n == index)
                {
                    valid = false;
                    Removed.Handler();
                }
            });
            list.Cleared.Add(() =>
            {
                valid = false;
                Removed.Handler();
            });
        }

        private R Validate<R>(Func<R> a)
        {
            if (valid)
            {
                return a();
            }
            else
            {
                throw new InvalidOperationException("This BindListPointer points to an element which has been removed from the list.");
            }
        }

        private void ValidateVoid(Action a)
        {
            if (valid)
            {
                a();
            }
            else
            {
                throw new InvalidOperationException("This BindListPointer points to an element which has been removed from the list.");
            }
        }

        public bool IsLast()
        {
            return Validate(() => index == list.Count - 1);
        }

        public BindListPointer<T> Next()
        {
            return Validate(() => index == list.Count - 1 ? null : new BindListPointer<T>(list, index + 1));
        }

        public void Remove()
        {
            ValidateVoid(() => list.RemoveAt(index));
        }
    }
}

