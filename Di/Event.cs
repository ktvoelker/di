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
namespace Di
{
    public abstract class Event<T>
    {
        public T Handler
        {
            get;
            protected set;
        }

        public abstract void Add(T f);

        public abstract void Remove(T f);
    }

    public class Event0 : Event<Action>
    {
        public Event0()
        {
            Handler = () => { return; };
        }

        public override void Add(Action f)
        {
            Handler += f;
        }

        public override void Remove(Action f)
        {
            Handler -= f;
        }
    }

    public class Event1<T> : Event<Action<T>>
    {
        public Event1()
        {
            Handler = (x) => { return; };
        }

        public override void Add(Action<T> f)
        {
            Handler += f;
        }

        public override void Remove(Action<T> f)
        {
            Handler -= f;
        }
    }

    public class Event2<T, U> : Event<Action<T, U>>
    {
        public Event2()
        {
            Handler = (x, y) => { return; };
        }

        public override void Add(Action<T, U> f)
        {
            Handler += f;
        }

        public override void Remove(Action<T, U> f)
        {
            Handler -= f;
        }
    }
}

