//  
//  Input.cs
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
using Gdk;

namespace Di.Controller
{
    public partial class Main
    {
        private Model.Main model;

        private IList<Buffer> visibleBuffers;
        public IEnumerable<Buffer> VisibleBuffers
        {
            get { return visibleBuffers; }
        }

        public KeyMap CommandMode
        {
            get;
            set;
        }

        public KeyMap InsertMode
        {
            get;
            set;
        }

        public Main(Model.Main m)
        {
            model = m;
            
            // Command mode bindings
            CommandMode = new KeyMap();
            CommandMode.Add(Key.i, ModifierType.None, new Command.InsertMode());
            
            // Insert mode bindings
            InsertMode = new KeyMap();
            InsertMode.SetDefault(new Command.InsertKey());
			InsertMode.Add(Key.Return, ModifierType.None, new Command.InsertChar('\n'));
            InsertMode.Add(Key.Escape, ModifierType.None, new Command.CommandMode());
			
            visibleBuffers = new List<Buffer>();
            if (model.Buffers.Count() > 0)
            {
                visibleBuffers.Add(new Buffer(CommandMode, InsertMode, model.Buffers.Item(0)));
            }
        }
    }
}

