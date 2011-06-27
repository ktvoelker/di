//  
//  Window.cs
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
using System.Text;
using Gtk;
using Gdk;
using Karl;
namespace Di.Controller
{
    public class Window
    {
        public readonly Main Controller;

        public readonly Bind<Model.Buffer> Model;

        public static readonly IList<WindowMode> DefaultMode = new List<WindowMode>();

        public BindSet<WindowMode> CurrentMode
        {
            get;
            private set;
        }

        private static readonly KeyMap EmptyKeyMap;

        private static readonly ISet<Gdk.Key> IgnoreKeys;

        public Event1<CharIter> CursorMovedByCommand = new Event1<CharIter>();

        static Window()
        {
            EmptyKeyMap = new KeyMap();
            IgnoreKeys = new HashSet<Gdk.Key>();
            IgnoreKeys.Add(Gdk.Key.Shift_L);
            IgnoreKeys.Add(Gdk.Key.Shift_R);
            IgnoreKeys.Add(Gdk.Key.Shift_Lock);
            IgnoreKeys.Add(Gdk.Key.Meta_L);
            IgnoreKeys.Add(Gdk.Key.Meta_R);
            IgnoreKeys.Add(Gdk.Key.Control_L);
            IgnoreKeys.Add(Gdk.Key.Control_R);
            IgnoreKeys.Add(Gdk.Key.Super_L);
            IgnoreKeys.Add(Gdk.Key.Super_R);
            IgnoreKeys.Add(Gdk.Key.Menu);
        }

        private KeyMap CurrentKeyMap = EmptyKeyMap;

        public readonly CommandParser Parser;

        public Window(Main _controller, Model.Buffer _model)
        {
            Controller = _controller;
            Model = new Bind<Model.Buffer>(_model);
            CurrentMode = new BindSet<WindowMode>();
            CurrentMode.Changed.Add(() =>
            {
                CurrentKeyMap = CurrentMode.ToList().FoldLeft(EmptyKeyMap, (a, b) => a + b.KeyMap);
            });
            DefaultMode.ForEach(CurrentMode.Add);
            Parser = new CommandParser();
        }

        public void KeyPressedHandler(EventKey e)
        {
            if (!IgnoreKeys.Contains(e.Key))
            {
                Parser.Parse(CurrentKeyMap.Lookup(e).Select(a => { return new UnparsedCommand(a, e.KeyValue); }));
                // TODO alert the user if there were any invalid sequences
                // TODO indicate the current state somewhere
                Model.Value.IncrUserAction();
                try
                {
                    Parser.Commands.ForEach(c => c.Execute(this));
                }
                finally
                {
                    Model.Value.DecrUserAction();
                }
                Parser.Commands.Clear();
            }
        }

        public void PlaceCursorKeepVisible(CharIter i)
        {
            Model.Value.PlaceCursor(i.GtkIter);
            CursorMovedByCommand.Handler(i);
        }
    }
}

