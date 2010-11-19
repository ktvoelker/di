//  
//  Command.cs
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
using Gtk;
namespace Di.Controller
{
    public struct Movement
    {
        public TextIter CursorStart, CursorEnd, RangeStart, RangeEnd;
    }

    public interface ICommand
    {
    }

    public abstract class LoneCommand : ICommand
    {
        public abstract void Execute(Buffer b);
    }

    /// <summary>
    /// A RepeatCommand is like a LoneCommand but can sensibly be repeated multiple times in sequence.
    /// </summary>
    public abstract class RepeatCommand : LoneCommand
    {
        public virtual RepeatCommand Repeat(uint count)
        {
            return new RepeatedCommand(this, count);
        }

        private class RepeatedCommand : RepeatCommand
        {
            private RepeatCommand _cmd;
            private uint _count;

            public RepeatedCommand(RepeatCommand cmd, uint count)
            {
                _cmd = cmd;
                _count = count;
            }

            public override void Execute(Buffer b)
            {
                for (uint i = 0; i < _count; ++i)
                {
                    _cmd.Execute(b);
                }
            }
        }
    }

    /// <summary>
    /// A MoveCommand is like a RepeatCommand but can also be the argument to a RangeCommand.
    /// </summary>
    public abstract class MoveCommand : RepeatCommand
    {
        public override void Execute(Buffer b)
        {
            b.GtkTextBuffer.PlaceCursor(Evaluate(b).CursorEnd);
        }

        public abstract Movement Evaluate(Buffer b);

        public new MoveCommand Repeat(uint count)
        {
            return new RepeatedCommand(this, count);
        }

        private class RepeatedCommand : MoveCommand
        {
            private MoveCommand _cmd;
            private uint _count;

            public RepeatedCommand(MoveCommand cmd, uint count)
            {
                _cmd = cmd;
                _count = count;
            }

            public override Movement Evaluate(Buffer b)
            {
                // TODO handle zero repetitions case
                var movement = _cmd.Evaluate(b);
                for (uint i = 1; i < _count - 1; ++i)
                {
                    _cmd.Evaluate(b);
                }
                if (_count >= 1)
                {
                    var lastMovement = _cmd.Evaluate(b);
                    movement.CursorEnd = lastMovement.CursorEnd;
                    movement.RangeEnd = lastMovement.RangeEnd;
                }
                return movement;
            }
        }
    }

    public abstract class RangeCommand : ICommand
    {
        private class CompleteRangeCommand : LoneCommand
        {
            private RangeCommand _range;
            private MoveCommand _move;

            public CompleteRangeCommand(RangeCommand range, MoveCommand move)
            {
                _range = range;
                _move = move;
            }

            public override void Execute(Buffer b)
            {
                _range.Execute(b, _move);
            }
        }

        public LoneCommand Complete(MoveCommand move)
        {
            return new CompleteRangeCommand(this, move);
        }

        public void Execute(Buffer b, MoveCommand move)
        {
            var movement = move.Evaluate(b);
            Execute(b, movement.RangeStart, movement.RangeEnd);
        }

        public abstract void Execute(Buffer b, TextIter start, TextIter end);
    }

    /// <summary>
    /// A NumCommand updates the current command number. The next non-NumCommand must be either a MoveCommand or a RepeatCommand.
    /// </summary>
    public class NumCommand : ICommand
    {
    }
}

