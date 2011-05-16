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

namespace Di.Model
{
    using UndoStack = TextStack<UndoElem, Buffer>;

    public class Buffer : Gtk.TextBuffer
    {
        private static Gtk.TextTagTable tags = new Gtk.TextTagTable();

        public readonly Bind<bool> HasUnsavedChanges = new Bind<bool>(false);

        public File File
        {
            get;
            private set;
        }

        public readonly UndoStack UndoStack, RedoStack;

        private int userActionDepth = 0;

        private UndoElem userAction = null;

        private bool ignoreChanges = false;

        public Buffer(File _file, UndoStack _undo, UndoStack _redo) : base(tags)
        {
            /**
             * Prepare for undo/redo.
             */
            UndoStack = _undo;
            RedoStack = _redo;
            UndoStack.Applied.Add(RedoStack.Push);
            RedoStack.Applied.Add(UndoStack.Push);

            /**
             * Read the file.
             */
            File = _file;
            var input = File.Info.OpenText();
            IgnoreChanges(() => InsertAtCursor(input.ReadToEnd()));
            input.Close();
            PlaceCursor(GetIterAtOffset(0));
            Changed += (o, a) =>
            {
                HasUnsavedChanges.Value = true;
            };
        }

        protected override void OnInsertText(Gtk.TextIter pos, string text)
        {
            if (!ignoreChanges)
            {
                AddUndoElem(new UndoElem(text, new CharIter(pos), UndoElem.ActionType.Add));
            }
            base.OnInsertText(pos, text);
        }

        protected override void OnDeleteRange(Gtk.TextIter start, Gtk.TextIter end)
        {
            if (!ignoreChanges)
            {
                AddUndoElem(new UndoElem(new Range(start, end).Chars, start, UndoElem.ActionType.Remove));
            }
            base.OnDeleteRange(start, end);
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

        private void AddUndoElem(UndoElem v)
        {
            if (userAction != null)
            {
                if (userAction.MergeWith(v))
                {
                    return;
                }
                else
                {
                    UndoStack.Push(userAction);
                    userAction = null;
                }
            }
            if (userActionDepth > 0)
            {
                userAction = v;
            }
            else
            {
                UndoStack.Push(v);
            }
        }

        public void IgnoreChanges(Action a)
        {
            var prev = ignoreChanges;
            ignoreChanges = true;
            try
            {
                a();
            }
            finally
            {
                ignoreChanges = prev;
            }
        }
    }
}

