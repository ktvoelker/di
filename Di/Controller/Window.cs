//  
//  Buffer.cs
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
using System.Text;
using Gtk;
using Gdk;
namespace Di.Controller
{
    public class Window
    {
        public readonly Main Controller;

        private Model.Buffer model;
        public TextBuffer GtkTextBuffer
        {
            get { return model; }
        }

        private WindowMode CommandMode;
        private WindowMode InsertMode;

        public Bind<Window, WindowMode> CurrentMode { get; private set; }
        private IList<UnparsedCommand> CurrentCommand;

        public Window(Main _controller, KeyMap _commandModeMap, KeyMap _insertModeMap, Model.Buffer _model)
        {
            Controller = _controller;
            model = _model;
            CommandMode = new WindowMode() { Name = "Command", KeyMap = _commandModeMap };
            InsertMode = new WindowMode() { Name = "Insert", KeyMap = _insertModeMap };
            CurrentMode = new Bind<Window, WindowMode>(this, CommandMode);
            CurrentCommand = new List<UnparsedCommand>();
        }

        public void KeyPressedHandler(EventKey e)
        {
			CurrentMode.Value.KeyMap.Lookup(e).ForEach(a =>
            {
                CurrentCommand.Add(new UnparsedCommand(a, e.KeyValue));
            });
            var result = new ParseResult(CurrentCommand);
            // TODO alert the user if there were any invalid sequences
            // TODO indicate the current state somewhere
            result.Commands.ForEach(c => c.Execute(this));
        }

        private void EnterMode(WindowMode b)
        {
            CurrentMode.Value = b;
        }

        public void EnterCommandMode()
        {
            EnterMode(CommandMode);
        }

        public void EnterInsertMode()
        {
            EnterMode(InsertMode);
        }
    }
}

