//  
//  Event.cs
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
using System.Collections.Generic;
namespace Di
{
    public enum EventPriority
    {
        ControllerHigh,
        ControllerLow,
        View,
        Default
    }

    public abstract class Event<T> where T : class
    {
        private struct PrioT : IComparable<PrioT>
        {
            public T elem;
            public EventPriority prio;

            public int CompareTo(PrioT other)
            {
                return prio < other.prio ? -1 : (prio > other.prio ? 1 : 0);
            }
        }

        private SortedSet<PrioT> handlers = new SortedSet<PrioT>(new CompareComparable<PrioT>());

        public T Handler
        {
            get;
            protected set;
        }

        private bool cancelled = false;

        public void BaseHandler(Func<T, Action> f)
        {
            cancelled = false;
            foreach (var pt in handlers)
            {
                f(pt.elem)();
                if (cancelled)
                {
                    cancelled = false;
                    break;
                }
            }
        }

        public void Add(EventPriority p, T f)
        {
            handlers.Add(new PrioT() { elem = f, prio = p });
        }

        public void Add(T f)
        {
            Add(EventPriority.Default, f);
        }

        public void Remove(T f)
        {
            handlers.RemoveWhere(pt => pt.elem == f);
        }

        public void Cancel()
        {
            cancelled = true;
        }
    }

    public class Event0 : Event<Action>
    {
        public Event0()
        {
            Handler = () => BaseHandler(a => a);
        }
    }

    public class Event1<T> : Event<Action<T>>
    {
        public Event1()
        {
            Handler = (x) => BaseHandler(a => a.Apply(x));
        }
    }

    public class Event2<T, U> : Event<Action<T, U>>
    {
        public Event2()
        {
            Handler = (x, y) => BaseHandler(a => a.Apply(x, y));
        }
    }

    public class Event3<T, U, V> : Event<Action<T, U, V>>
    {
        public Event3()
        {
            Handler = (x, y, z) => { return; };
        }
    }
}

