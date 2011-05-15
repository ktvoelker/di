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
using System.IO;
using Gtk;

namespace Di.Model
{
    using UndoStack = TextStack<UndoElem, Buffer>;

    public class Buffer : TextBuffer
    {
        private static TextTagTable tags = new TextTagTable();

        public readonly Bind<bool> HasUnsavedChanges = new Bind<bool>(false);

        public File File
        {
            get;
            private set;
        }

        public readonly UndoStack UndoStack, RedoStack;

        private int userActionDepth = 0;

        private UndoElem userAction = null;

        public Buffer(File _file, UndoStack _undo, UndoStack _redo) : base(tags)
        {
            /**
             * Read the file.
             */
            File = _file;
            var input = File.Info.OpenText();
            InsertAtCursor(input.ReadToEnd());
            input.Close();
            PlaceCursor(GetIterAtOffset(0));
            Changed += (o, a) =>
            {
                HasUnsavedChanges.Value = true;
            };

            /**
             * Prepare for undo/redo.
             */
            UndoStack = _undo;
            RedoStack = _redo;
            UndoStack.Applied.Add(RedoStack.Push);
            RedoStack.Applied.Add(UndoStack.Push);
            Changed += delegate(object sender, EventArgs e) {
                if (userAction != null)
                {
                    if (CombineUserAction(e))
                    {
                        return;
                    }
                    else
                    {
                        UndoStack.Push(userAction);
                        userAction = null;
                    }
                }
                var v = CreateUndoElem(e);
                if (userActionDepth > 0)
                {
                    userAction = v;
                }
                else
                {
                    UndoStack.Push(v);
                }
            };
        }

        public Buffer(File _file) : this(_file, new UndoStack(), new UndoStack())
        {
            // empty
        }

        public void InsertAtCursor(char c)
        {
            InsertAtCursor(string.Format("{0}", c));
        }

        public void Save()
        {
            if (HasUnsavedChanges && File != null)
            {
                var output = new StreamWriter(File.Info.Open(FileMode.Truncate, FileAccess.Write));
                output.Write(Text);
                output.Close();
                HasUnsavedChanges.Value = false;
            }
        }

        public void IncrUserAction()
        {
            if (userActionDepth < Int32.MaxValue)
            {
                ++userActionDepth;
            }
        }

        public void DecrUserAction()
        {
            if (userActionDepth > 0)
            {
                --userActionDepth;
            }
            if (userActionDepth == 0 && userAction != null)
            {
                UndoStack.Push(userAction);
                userAction = null;
            }
        }

        private bool CombineUserAction(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private UndoElem CreateUndoElem(EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

