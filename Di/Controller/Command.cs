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
namespace Di.Controller
{
    public struct Movement
    {
        public Range CursorRange, ActionRange;
    }

    public interface ICommand
    {
    }

    public interface IResultCommand : ICommand
    {
        void Execute(Window b);
    }

    public abstract class InterruptCommand : IResultCommand
    {
        public abstract void Execute(Window b);
    }

    public abstract class LoneCommand : IResultCommand
    {
        public abstract void Execute(Window b);
    }

    /// <summary>
    /// A RepeatCommand is like a LoneCommand but can sensibly be repeated multiple times in sequence.
    /// </summary>
    public abstract class RepeatCommand : LoneCommand
    {
        public RepeatCommand Repeat(uint count)
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

            public override void Execute(Window b)
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
    public abstract class MoveCommand : LoneCommand
    {
        public override void Execute(Window b)
        {
            b.PlaceCursorKeepVisible(Evaluate(b, b.Model.GetCursorIter()).CursorRange.End);
        }

        public abstract Movement Evaluate(Window b, CharIter start);

        public MoveCommand Repeat(uint count)
        {
            return new RepeatedCommand(this, count);
        }

        private class RepeatedCommand : MoveCommand
        {
            private MoveCommand cmd;
            private uint count;

            public RepeatedCommand(MoveCommand _cmd, uint _count)
            {
                cmd = _cmd;
                count = _count;
            }

            public override Movement Evaluate(Window b, CharIter start)
            {
                if (count == 0)
                {
                    return new Movement()
                    {
                        CursorRange = new Range(start, start),
                        ActionRange = new Range(start, start)
                    };
                }
                Movement firstMovement = cmd.Evaluate(b, start);
                Movement lastMovement = firstMovement;
                for (uint i = 1; i < count; ++i)
                {
                    lastMovement = cmd.Evaluate(b, lastMovement.CursorRange.End);
                }
                return new Movement()
                {
                    CursorRange = new Range(firstMovement.CursorRange.Start, lastMovement.CursorRange.End),
                    ActionRange = new Range(firstMovement.ActionRange.Start, lastMovement.ActionRange.End)
                };
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

            public override void Execute(Window b)
            {
                _range.Execute(b, _move);
            }
        }

        public LoneCommand Complete(MoveCommand move)
        {
            return new CompleteRangeCommand(this, move);
        }

        public void Execute(Window b, MoveCommand move)
        {
            var movement = move.Evaluate(b, b.Model.GetCursorIter());
            Execute(b, movement.ActionRange);
        }

        public abstract void Execute(Window b, Range r);
    }

    /// <summary>
    /// A NumCommand updates the current command number. The next non-NumCommand must be either a MoveCommand or a RepeatCommand.
    /// </summary>
    public class NumCommand : ICommand
    {
    }
}

