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
using System.Linq;
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
        private string key;

        public AddWindowMode(string _key)
        {
            key = _key;
        }

        public override void Execute(Window b)
        {
            b.Controller.WindowModes[key].ForEach(b.CurrentMode.Add);
        }
    }

    public class RemoveWindowMode : LoneCommand
    {
        private string key;

        public RemoveWindowMode(string _key)
        {
            key = _key;
        }

        public override void Execute(Window b)
        {
            b.Controller.WindowModes[key].ForEach(m => { b.CurrentMode.Remove(m); });
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

    public abstract class FileCommand : LoneCommand
    {
        public static Action<Model.File> InNewWindow(Main ctl)
        {
            return file =>
            {
                ctl.Windows.Current = ctl.FindOrCreateWindow(file);
            };
        }

        public static Action<Model.File> InFocusedWindow(Main ctl)
        {
            return file =>
            {
                var window = ctl.FindWindow(file);
                if (window == null)
                {
                    ctl.Windows.Current.Model.Value = ctl.Model.FindOrCreateBuffer(file);
                }
                else
                {
                    ctl.Windows.Current = window;
                }
            };
        }

        public static void OpenFile(Main ctl, Func<Main, Action<Model.File>> handler, bool allowEscape)
        {
            var chooser = new FsChooser<Model.File>(() => ctl.Model.Files, "Choose a file", allowEscape);
            chooser.Choose.Add(handler(ctl));
            ctl.BeginTask.Handler(chooser);
        }

        public static void NewFileInDirectory(Main ctl, Func<Main, Action<Model.File>> fileHandler, Model.Directory dir, string initDirQuery)
        {
            var fileChooser = new NewFileChooser(dir);
            fileChooser.ChooseFile.Add(fileHandler(ctl));
            fileChooser.Cancel.Add(() =>
            {
                NewFile(ctl, fileHandler, initDirQuery);
            });
            ctl.BeginTask.Handler(fileChooser);
        }

        public static void NewFile(Main ctl, Func<Main, Action<Model.File>> fileHandler, string initDirQuery)
        {
            var dirChooser = new FsChooser<Model.Directory>(() => ctl.Model.Directories, "Choose a directory", true);
            dirChooser.Choose.Add(EventPriority.ControllerHigh, dir => NewFileInDirectory(ctl, fileHandler, dir, dirChooser.Query));
            dirChooser.Query = initDirQuery;
            ctl.BeginTask.Handler(dirChooser);
        }
    }

    public class OpenFileInNewWindow : FileCommand
    {
        public override void Execute(Window b)
        {
            OpenFile(b.Controller, InNewWindow, true);
        }
    }

    public class OpenFile : FileCommand
    {
        public override void Execute(Window b)
        {
            OpenFile(b.Controller, InFocusedWindow, true);
        }
    }

    public class NewFileInNewWindow : FileCommand
    {
        public override void Execute(Window b)
        {
            NewFile(b.Controller, InNewWindow, "");
        }
    }

    public class NewFile : FileCommand
    {
        public override void Execute(Window b)
        {
            NewFile(b.Controller, InFocusedWindow, "");
        }
    }

    public class CloseWindow : LoneCommand
    {
        public override void Execute(Window b)
        {
            b.Controller.Windows.Remove(b);
        }
    }

    public class FocusLeft : RepeatCommand
    {
        public override void Execute(Window b)
        {
            b.Controller.Windows.Previous();
        }
    }

    public class FocusRight : RepeatCommand
    {
        public override void Execute(Window b)
        {
            b.Controller.Windows.Next();
        }
    }

    public class Save : LoneCommand
    {
        public override void Execute(Window b)
        {
            b.Controller.Save();
        }
    }
}

