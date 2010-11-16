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
        private IList<Model.CommandAtom> CurrentCommand;
        private IList<char> CurrentCommandKeyValues;

        public Buffer(Model.Main _mainModel, Model.Buffer _model)
        {
            model = _model;
            CommandMode = new BufferMode() { Name = "Command", KeyMap = _mainModel.CommandMode };
            InsertMode = new BufferMode() { Name = "Insert", KeyMap = _mainModel.InsertMode };
            CurrentMode = CommandMode;
            CurrentCommand = new List<Model.CommandAtom>();
            CurrentCommandKeyValues = new List<char>();
        }

        public void KeyPressedHandler(EventKey e)
        {
            var ch = (char) e.KeyValue;
            CurrentMode.KeyMap[e].Atoms.ForEach(a =>
            {
                CurrentCommand.Add(a);
                CurrentCommandKeyValues.Add(ch);
            });
            ParseCommands();
        }

        private void ParseCommands()
        {
            while (CurrentCommand.Count > 0)
            {
                var a = CurrentCommand[0];
                if (a == Model.CommandAtom.Ignore)
                {
                    CurrentCommand.RemoveAt(0);
                    CurrentCommandKeyValues.RemoveAt(0);
                }
                else if (a == Model.CommandAtom.CommandMode)
                {
                    CurrentCommand.RemoveAt(0);
                    CurrentCommandKeyValues.RemoveAt(0);
                    CurrentMode = CommandMode;
                }
                else if (a == Model.CommandAtom.InsertMode)
                {
                    CurrentCommand.RemoveAt(0);
                    CurrentCommandKeyValues.RemoveAt(0);
                    CurrentMode = InsertMode;
                }
                else if (a == Model.CommandAtom.InsertKey)
                {
                    CurrentCommand.RemoveAt(0);
                    model.InsertAtCursor(CurrentCommandKeyValues[0]);
                    CurrentCommandKeyValues.RemoveAt(0);
                }
            }
        }
    }
}

