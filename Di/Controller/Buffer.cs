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
    public class Buffer
    {
        private Model.Buffer model;
        public TextBuffer GtkTextBuffer
        {
            get { return model; }
        }

        private BufferMode CommandMode;
        private BufferMode InsertMode;

        private BufferMode CurrentMode;
        private IList<Model.UnparsedCommand> CurrentCommand;

        public Buffer(Model.Main _mainModel, Model.Buffer _model)
        {
            model = _model;
            CommandMode = new BufferMode() { Name = "Command", KeyMap = _mainModel.CommandMode };
            InsertMode = new BufferMode() { Name = "Insert", KeyMap = _mainModel.InsertMode };
            CurrentMode = CommandMode;
            CurrentCommand = new List<Model.UnparsedCommand>();
        }

        public void KeyPressedHandler(EventKey e)
        {
            CurrentMode.KeyMap.Lookup(e).ForEach(a =>
            {
                CurrentCommand.Add(new Model.UnparsedCommand(a, e.KeyValue));
            });
            // TODO parse the commands and execute the results
        }
    }
}

