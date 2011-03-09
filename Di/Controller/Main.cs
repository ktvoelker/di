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
        public readonly Model.Main Model;

        private readonly BindList<Window> windows;
        public readonly ReadOnlyCollection<Window> Windows;
        public readonly BindList<Window>.Events WindowsEvents;

        public readonly Bind<Window> FocusedWindow = new Bind<Window>(null);

        public readonly ReadOnlyCollection<WindowMode> WindowModes;

        public readonly Event1<FileChooser> BeginFileChooser = new Event1<FileChooser>();

        public readonly Event1<FileChooser> EndFileChooser = new Event1<FileChooser>();

        public readonly Event0 CancelFileChooser = new Event0();

        public Main(Model.Main m)
        {
            Model = m;
            
            var windowModes = new List<WindowMode>();
            WindowModes = new ReadOnlyCollection<WindowMode>(windowModes);

            // Command mode bindings (0)
            var commandMode = new KeyMap() { Priority = 5 };
            commandMode.Add(Key.i, new Command.ClearWindowMode(), new Command.AddWindowMode(1));
            commandMode.Add(Key.h, new Command.Down());
            commandMode.Add(Key.t, new Command.Up());
            commandMode.Add(Key.d, new Command.Left());
            commandMode.Add(Key.n, new Command.Right());
            commandMode.Add(Key.Down, new Command.Down());
            commandMode.Add(Key.Up, new Command.Up());
            commandMode.Add(Key.Left, new Command.Left());
            commandMode.Add(Key.Right, new Command.Right());
            commandMode.Add(Key.w, new Command.ClearWindowMode(), new Command.AddWindowMode(4), new Command.AddWindowMode(3));
            windowModes.Add(new WindowMode { Name = "Command", KeyMap = commandMode });
            
            // Insert mode bindings (1)
            var insertMode = new KeyMap();
            insertMode.SetDefault(new Command.InsertKey());
            insertMode.Add(Key.Return, new Command.InsertChar('\n'));
            insertMode.Add(Key.Escape,
                           new Command.DiscardInput(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode(0),
                           new Command.AddWindowMode(2),
                           new Command.AddWindowMode(3));
            insertMode.Add(Key.BackSpace, new Command.Delete(), new Command.Backspace());
            insertMode.Add(Key.Tab, new Command.Tab());
            insertMode.Add(Key.Down, new Command.Down());
            insertMode.Add(Key.Up, new Command.Up());
            insertMode.Add(Key.Left, new Command.Left());
            insertMode.Add(Key.Right, new Command.Right());
            insertMode.Add(Key.Delete, new Command.Delete(), new Command.Right());
            windowModes.Add(new WindowMode { Name = "Insert", KeyMap = insertMode });

            // Number mode bindings (2)
            var numberMode = new KeyMap();
            numberMode.Add(Key.Key_0, new NumCommand());
            numberMode.Add(Key.Key_1, new NumCommand());
            numberMode.Add(Key.Key_2, new NumCommand());
            numberMode.Add(Key.Key_3, new NumCommand());
            numberMode.Add(Key.Key_4, new NumCommand());
            numberMode.Add(Key.Key_5, new NumCommand());
            numberMode.Add(Key.Key_6, new NumCommand());
            numberMode.Add(Key.Key_7, new NumCommand());
            numberMode.Add(Key.Key_8, new NumCommand());
            numberMode.Add(Key.Key_9, new NumCommand());
            windowModes.Add(new WindowMode { Name = "Number", Hidden = true, KeyMap = numberMode });

            // Common bindings (3)
            var commonMode = new KeyMap();
            commonMode.Add(Key.Escape, new Command.DiscardInput());
            windowModes.Add(new WindowMode { Name = "Common", Hidden = true, KeyMap = commonMode });

            // Window mode bindings (4)
            var windowMode = new KeyMap();
            windowMode.Add(Key.a,
                           new Command.CreateAndFocusWindow(),
                           new Command.ClearWindowMode(),
                           new Command.AddWindowMode(0),
                           new Command.AddWindowMode(2),
                           new Command.AddWindowMode(3));
            windowModes.Add(new WindowMode { Name = "Window", KeyMap = windowMode });
            
            windows = new BindList<Window>();
            if (Model.Buffers.HasAny())
            {
                var window = new Window(this, Model.Buffers.Item(0));
                windows.Add(window);
                FocusedWindow.Value = window;
            }
            Windows = new ReadOnlyCollection<Window>(windows);
            WindowsEvents = windows.Event;
        }

        public Window CreateWindow()
        {
            var window = new Window(this, Model.CreateBuffer());
            windows.Add(window);
            return window;
        }

        public Window CreateWindow(Di.Model.ProjectFile file)
        {
            var window = new Window(this, Model.CreateBuffer(file));
            windows.Add(window);
            return window;
        }

        public Window FindOrCreateWindow(Di.Model.ProjectFile file)
        {
            foreach (var window in Windows)
            {
                if (window.Model.File == file)
                {
                    return window;
                }
            }
            return CreateWindow(file);
        }
    }
}

