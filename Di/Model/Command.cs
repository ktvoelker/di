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
namespace Di.Model
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
        public abstract void Execute(Main m);
    }

    /// <summary>
    /// A RepeatCommand is like a LoneCommand but can sensibly be repeated multiple times in sequence.
    /// </summary>
    public abstract class RepeatCommand : LoneCommand
    {
        public RepeatCommand Repeat(int count)
        {
            return new RepeatedCommand(this, count);
        }

        private class RepeatedCommand : RepeatCommand
        {
            private LoneCommand _cmd;
            private int _count;

            public RepeatedCommand(RepeatCommand cmd, int count)
            {
                _cmd = cmd;
                _count = count;
            }

            public override void Execute(Main m)
            {
                for (int i = 0; i < _count; ++i)
                {
                    _cmd.Execute(m);
                }
            }
        }
    }

    /// <summary>
    /// A MoveCommand is like a RepeatCommand but can also be the argument to a RangeCommand.
    /// </summary>
    public abstract class MoveCommand : RepeatCommand
    {
        public override void Execute(Main m)
        {
            throw new NotImplementedException();
        }

        public abstract Movement Evaluate(Main m);
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

            public override void Execute(Main m)
            {
                _range.Execute(m, _move);
            }
        }

        public LoneCommand Complete(MoveCommand move)
        {
            return new CompleteRangeCommand(this, move);
        }

        public void Execute(Main m, MoveCommand move)
        {
            var movement = move.Evaluate(m);
            Execute(m, movement.RangeStart, movement.RangeEnd);
        }

        public abstract void Execute(Main m, TextIter start, TextIter end);
    }

    /// <summary>
    /// A NumCommand updates the current command number. The next non-NumCommand must be either a MoveCommand or a RepeatCommand.
    /// </summary>
    public class NumCommand : ICommand
    {
    }
}

