//
//  KeyMap.cs
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
using Gdk;
namespace Di.Controller
{
    using IEC = IEnumerable<ICommand>;

    public struct KeyInput
    {
        public Key Base
        {
            get;
            set;
        }

        public ModifierType Modifiers
        {
            get;
            set;
        }

        public KeyInput(Key _base, ModifierType _modifiers) : this()
        {
            Base = _base;
            Modifiers = _modifiers;
        }

        public KeyInput(EventKey e) : this()
        {
            Base = e.Key;
            Modifiers = e.State;
        }
    }

    public class PrioCommand
    {
        public readonly sbyte Priority;

        public readonly IEC Command;

        public PrioCommand(sbyte p, IEC c)
        {
            Priority = p;
            Command = c;
        }

        public static PrioCommand operator +(PrioCommand a, PrioCommand b)
        {
            if (a == null && b == null)
            {
                return null;
            }
            else if (a == null)
            {
                return b;
            }
            else if (b == null)
            {
                return a;
            }
            else if (a.Priority > b.Priority)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
    }

    public class KeyMap
    {
        public PrioCommand Default
        {
            get;
            set;
        }

        private IDictionary<KeyInput, PrioCommand> _map;

        public KeyMap()
        {
            Default = new PrioCommand(sbyte.MinValue, new ICommand[] { new Command.Ignore() });
            _map = new Dictionary<KeyInput, PrioCommand>();
        }

        public void SetDefault(sbyte priority, IEC commands)
        {
            Default = new PrioCommand(priority, commands);
        }

        public void Add(Key _base, ModifierType _modifiers, sbyte priority, IEC command)
        {
            _map[new KeyInput(_base, _modifiers)] = new PrioCommand(priority, command);
        }

        public IEC Lookup(KeyInput key)
        {
            return _map.ContainsKey(key) ? _map[key].Command : Default.Command;
        }

        public IEC Lookup(EventKey e)
        {
            return Lookup(new KeyInput(e));
        }

        public static KeyMap operator +(KeyMap a, KeyMap b)
        {
            KeyMap result = new KeyMap();
            result.Default = a.Default + b.Default;
            a._map.Keys.Union(b._map.Keys).ForEach(k => result._map[k] = a._map.GetWithDefault(k, null) + b._map.GetWithDefault(k, null));
            return result;
        }
    }
}

