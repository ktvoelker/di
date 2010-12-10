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
    public class CommandMode : LoneCommand
    {
        public override void Execute(Buffer b)
        {
            b.EnterCommandMode();
        }
    }

    public class Down : MoveCommand
    {
        public override Movement Evaluate(Buffer b)
        {
            // TODO does CursorPosition return a char offset or a byte index?
            // The rest of the logic uses char offsets because we only want to work in whole chars.
            var cursorStart = b.GtkTextBuffer.GetCursorIter();
            var actionStart = cursorStart.LineStart;
            var actionEnd = actionStart.ForwardLines(1);
            var cursorEnd = actionEnd + (cursorStart - actionStart);
            return new Movement()
            {
                CursorRange = new Range(cursorStart, cursorEnd),
                ActionRange = new Range(actionStart, actionEnd)
            };
        }
    }

    public class Ignore : LoneCommand
    {
        public override void Execute(Buffer b)
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
		
		public override void Execute(Buffer b)
		{
			if (_buffer != null)
			{
            	b.GtkTextBuffer.InsertAtCursor(_buffer.ToString());
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
			return this;
		}
		
        public override void Execute(Buffer b)
		{
			throw new NotSupportedException();
		}
    }

    public class InsertMode : LoneCommand
    {
        public override void Execute(Buffer b)
        {
            b.EnterInsertMode();
        }
    }

    public class Delete : RangeCommand
    {
        public override void Execute(Buffer b, Range r)
        {
            b.GtkTextBuffer.Delete(r);
        }
    }

    public class Backspace : MoveCommand
    {
        public override Movement Evaluate(Buffer b)
        {
            CharIter start = b.GtkTextBuffer.GetCursorIter();
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
            return new Movement()
            {
                CursorRange = new Range(start, end),
                ActionRange = new Range(start, end)
            };
        }
    }
}

