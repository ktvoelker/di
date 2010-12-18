//  
//  Bind.cs
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
namespace Di
{
    public class Actor : IDisposable
    {
        private Action action;

        public Actor(Action _action)
        {
            action = _action;
        }

        public void Dispose()
        {
            action();
        }
    }

    public class Bind<P, V>
    {
        public delegate void ValueChanged(P p, V v);

        private ValueChanged _changed = (p, v) => { return; };

        public event ValueChanged Changed
        {
            add { _changed += value; }
            remove { _changed -= value; }
        }

        private P p;

        private V v;

        public V Value
        {
            get { return v; }

            set
            {
                v = value;
                _changed(p, v);
            }
        }

        public Bind(P _p)
        {
            p = _p;
        }

        public Bind(P _p, V _v) : this(_p)
        {
            v = _v;
        }

        public static implicit operator V(Bind<P, V> obj)
        {
            return obj.v;
        }

        public IDisposable WithChange()
        {
            return new Actor(() => _changed(p, v));
        }
    }
}

