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
using System.Collections.ObjectModel;
using Gdk;

namespace Di.Controller
{
    public partial class Main
    {
        private Model.Main model;

        private readonly BindList<Window> windows;
        public readonly ReadOnlyCollection<Window> Windows;
        public readonly BindList<Window>.Events WindowsEvents;

        public readonly Bind<Window> FocusedWindow = new Bind<Window>(null);

        public readonly ReadOnlyCollection<WindowMode> WindowModes;

        public Main(Model.Main m)
        {
            model = m;
            
            var windowModes = new List<WindowMode>();
            WindowModes = new ReadOnlyCollection<WindowMode>(windowModes);
            
            // Command mode bindings
            var commandMode = new KeyMap();
            commandMode.Add(Key.i, new Command.ClearWindowMode(), new Command.AddWindowMode(1));
            commandMode.Add(Key.h, new Command.Down());
            commandMode.Add(Key.t, new Command.Up());
            commandMode.Add(Key.d, new Command.Left());
            commandMode.Add(Key.n, new Command.Right());
            commandMode.Add(Key.Down, new Command.Down());
            commandMode.Add(Key.Up, new Command.Up());
            commandMode.Add(Key.Left, new Command.Left());
            commandMode.Add(Key.Right, new Command.Right());
            windowModes.Add(new WindowMode { Name = "Command", KeyMap = commandMode });
            
            // Insert mode bindings
            var insertMode = new KeyMap();
            insertMode.SetDefault(new Command.InsertKey());
            insertMode.Add(Key.Return, new Command.InsertChar('\n'));
            insertMode.Add(Key.Escape, new Command.ClearWindowMode(), new Command.AddWindowMode(0));
            insertMode.Add(Key.BackSpace, new Command.Delete(), new Command.Backspace());
            insertMode.Add(Key.Tab, new Command.Tab());
            insertMode.Add(Key.Down, new Command.Down());
            insertMode.Add(Key.Up, new Command.Up());
            insertMode.Add(Key.Left, new Command.Left());
            insertMode.Add(Key.Right, new Command.Right());
            insertMode.Add(Key.Delete, new Command.Delete(), new Command.Right());
            windowModes.Add(new WindowMode { Name = "Insert", KeyMap = insertMode });
            
            windows = new BindList<Window>();
            if (model.Buffers.HasAny())
            {
                var window = new Window(this, model.Buffers.Item(0));
                windows.Add(window);
                FocusedWindow.Value = window;
            }
            Windows = new ReadOnlyCollection<Window>(windows);
            WindowsEvents = windows.Event;
        }

        public Window CreateWindow()
        {
            var window = new Window(this, model.CreateBuffer());
            windows.Add(window);
            return window;
        }
    }
}

