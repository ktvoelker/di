//
//  Commands.cs
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
using System.Text;
namespace Di.Controller.Command
{
    public class DiscardInput : InterruptCommand
    {
        public override void Execute(Window b)
        {
            b.Parser.Reset();
        }
    }

    public class AddWindowMode : LoneCommand
    {
        private int index;

        public AddWindowMode(int _index)
        {
            index = _index;
        }

        public override void Execute(Window b)
        {
            b.CurrentMode.Add(b.Controller.WindowModes[index]);
        }
    }

    public class RemoveWindowMode : LoneCommand
    {
        private int index;

        public RemoveWindowMode(int _index)
        {
            index = _index;
        }

        public override void Execute(Window b)
        {
            b.CurrentMode.Remove(b.Controller.WindowModes[index]);
        }
    }

    public class ClearWindowMode : LoneCommand
    {
        public override void Execute(Window b)
        {
            b.CurrentMode.Clear();
        }
    }

    public class CurLine : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter cursorStart)
        {
            var cursorEnd = cursorStart;
            var actionStart = cursorStart.LineStart;
            var actionEnd = actionStart.ForwardLines(1);
            return new Movement { CursorRange = new Range(cursorStart, cursorEnd), ActionRange = new Range(actionStart, actionEnd) };
        }
    }

    public class Down : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter cursorStart)
        {
            var actionStart = cursorStart.LineStart;
            var actionEnd = actionStart.ForwardLines(2);
            var cursorEnd = cursorStart.ForwardLines(1);
            return new Movement { CursorRange = new Range(cursorStart, cursorEnd), ActionRange = new Range(actionStart, actionEnd) };
        }
    }

    public class Up : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter cursorStart)
        {
            var actionStart = cursorStart.LineStart.ForwardLines(1);
            var actionEnd = actionStart.BackwardLines(2);
            var cursorEnd = cursorStart.BackwardLines(1);
            return new Movement { CursorRange = new Range(cursorStart, cursorEnd), ActionRange = new Range(actionStart, actionEnd) };
        }
    }

    public class Left : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter start)
        {
            var range = new Range(start, start - 1);
            return new Movement { CursorRange = range, ActionRange = range };
        }
    }

    public class Right : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter start)
        {
            var range = new Range(start, start + 1);
            return new Movement { CursorRange = range, ActionRange = range };
        }
    }

    public class Tab : RepeatCommand
    {
        public override void Execute(Window b)
        {
            var cursor = b.Model.Value.GetCursorIter();
            b.Model.Value.InsertAtCursor((cursor - cursor.LineStart) % 2 == 0 ? "  " : " ");
        }
    }

    public class Ignore : RepeatCommand
    {
        public override void Execute(Window b)
        {
            // Empty
        }
    }

    public class InsertChar : RepeatCommand
    {
        private StringBuilder _buffer;

        public InsertChar(char val)
        {
            _buffer = new StringBuilder(1);
            _buffer.Append(val);
        }

        public override void Execute(Window b)
        {
            if (_buffer != null)
            {
                b.Model.Value.InsertAtCursor(_buffer.ToString());
            }
        }
    }

    public class InsertKey : RepeatCommand
    {
        public const char MinInsertableChar = ' ';
        public const char MaxInsertableChar = '~';

        public RepeatCommand SetKey(uint val)
        {
            if (val >= MinInsertableChar && val <= MaxInsertableChar)
            {
                return new InsertChar((char) val);
            }
            return new Ignore();
        }

        public override void Execute(Window b)
        {
            throw new NotSupportedException();
        }
    }

    public class Delete : RangeCommand
    {
        public override void Execute(Window b, Range r)
        {
            b.Model.Value.Delete(r);
        }
    }

    public class Backspace : MoveCommand
    {
        public override Movement Evaluate(Window b, CharIter start)
        {
            CharIter end = start;
            CharIter line = start.LineStart;
            if (start - line <= 1)
            {
                --end;
            }

            else
            {
                string xs = new Range(line, end).Chars.Trim();
                int n = xs == string.Empty ? 2 : 1;
                end -= n;
            }
            return new Movement { CursorRange = new Range(start, end), ActionRange = new Range(start, end) };
        }
    }

    public class OpenFileInNewWindow : LoneCommand
    {
        public override void Execute(Window b)
        {
            Action<Di.Model.File> handler = file =>
            {
                b.Controller.FocusedWindow.Value = b.Controller.FindOrCreateWindow(file);
            };
            b.Controller.BeginTask.Handler(new FsChooser<Model.File>(() => b.Controller.Model.Files, "Choose a file", handler));
        }
    }

    public class OpenFile : LoneCommand
    {
        public override void Execute(Window b)
        {
            Action<Model.File> handler = file =>
            {
                var window = b.Controller.FindWindow(file);
                if (window == null)
                {
                    b.Controller.FocusedWindow.Value.Model.Value = b.Controller.Model.FindOrCreateBuffer(file);
                }
                else
                {
                    b.Controller.FocusedWindow.Value = window;
                }
            };
            b.Controller.BeginTask.Handler(new FsChooser<Model.File>(() => b.Controller.Model.Files, "Choose a file", handler));
        }
    }

    public class CloseWindow : LoneCommand
    {
        public override void Execute(Window b)
        {
            b.Controller.Windows.Remove(b);
        }
    }
}

