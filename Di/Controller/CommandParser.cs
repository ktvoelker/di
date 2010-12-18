//  
//  CommandParser.cs
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Di.Controller
{
    [Flags]
    public enum ParserExpectation : byte
    {
        Lone = 1,
        Move = 2,
        Range = 4,
        Repeat = 8,
        Num = 16,
        SameRange = 32,
        Any = Lone | Move | Range | Repeat | Num,
        AfterRange = SameRange | Move | Num,
        AfterRangeNum = Move | Num,
        AfterNum = Move | Repeat | Num,
        AfterMove = Any,
        AfterRepeat = Any,
        AfterLone = Any,
        AfterSameRange = Any
    }

    public class CommandParser
    {
        public readonly IList<LoneCommand> Commands;

        public ParserExpectation State
        {
            get;
            private set;
        }

        private readonly IList<ReadOnlyCollection<UnparsedCommand>> skipped;
        public readonly ReadOnlyCollection<ReadOnlyCollection<UnparsedCommand>> Skipped;

        private readonly IList<UnparsedCommand> atoms;
        private int i;
        private RangeCommand rangeCmd;
        private uint count;

        public CommandParser()
        {
            Commands = new List<LoneCommand>();
            State = ParserExpectation.Any;
            skipped = new List<ReadOnlyCollection<UnparsedCommand>>();
            Skipped = new ReadOnlyCollection<ReadOnlyCollection<UnparsedCommand>>(skipped);
            atoms = new List<UnparsedCommand>();
            i = 0;
            rangeCmd = null;
            count = 0;
        }

        public void Parse(IEnumerable<UnparsedCommand> _atoms)
        {
            _atoms.ForEach(a => atoms.Add(a));

            while (i < atoms.Count)
            {
                var atom = atoms[i].Atom;
                var input = atoms[i].Input;
                MoveCommand moveCmd;
                switch (State)
                {
                    // Initial state
                    case ParserExpectation.Any:
                        // Lone, repeat, or move command
                        var loneCmd = atom as LoneCommand;
                        if (loneCmd != null)
                        {
                            Commands.Add(SetKey(loneCmd, input));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Range command
                        rangeCmd = atom as RangeCommand;
                        if (rangeCmd != null)
                        {
                            ++i;
                            State = ParserExpectation.AfterRange;
                            continue;
                        }
                        // Num command
                        ParseNumCommand(atom, input);
                        break;

                    // After a range command
                    case ParserExpectation.AfterRange:
                        // Same range command
                        var sameRangeCmd = atom as RangeCommand;
                        if (sameRangeCmd != null && sameRangeCmd.GetType() == rangeCmd.GetType())
                        {
                            Commands.Add(rangeCmd.Complete(new Command.CurLine()));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Move command
                        moveCmd = atom as MoveCommand;
                        if (moveCmd != null)
                        {
                            Commands.Add(rangeCmd.Complete(moveCmd));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Num command
                        ParseNumCommand(atom, input);
                        break;

                    // After a num that was after a range
                    case ParserExpectation.AfterRangeNum:
                        // Move command
                        moveCmd = atom as MoveCommand;
                        if (moveCmd != null)
                        {
                            Commands.Add(rangeCmd.Complete(moveCmd.Repeat(count)));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Num command
                        ParseNumCommand(atom, input);
                        break;

                    // After a num command
                    case ParserExpectation.AfterNum:
                        // Move command
                        moveCmd = atom as MoveCommand;
                        if (moveCmd != null)
                        {
                            Commands.Add(moveCmd.Repeat(count));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Repeat command
                        var repCmd = atom as RepeatCommand;
                        if (repCmd != null)
                        {
                            Commands.Add(SetKey(repCmd, input).Repeat(count));
                            ++i;
                            Reset();
                            continue;
                        }
                        // Num command
                        ParseNumCommand(atom, input);
                        break;

                    default:
                        Skip();
                        Reset();
                        break;
                }
            }
        }
		
		private T SetKey<T>(T cmd, uint input) where T : class
		{
			var insKeyCmd = cmd as Command.InsertKey;
			if (insKeyCmd != null)
			{
				return insKeyCmd.SetKey(input) as T;
			}
			return cmd;
		}

        private void ParseNumCommand(ICommand cmd, uint input)
        {
            var num = cmd as NumCommand;
            if (num != null)
            {
                ++i;
                uint val = input - (uint) '0';
                if (val >= 0 && val <= 9)
                {
                    count *= 10;
                    count += val;
                    State = ParserExpectation.AfterNum;
                    return;
                }
            }
            Skip();
            Reset();
        }

        /// <summary>
        /// Add the processed input to the list failed input sequences.
        /// </summary>
        private void Skip()
        {
            var skip = new List<UnparsedCommand>();
            skip.AddRange(atoms.Take(i));
            skipped.Add(new ReadOnlyCollection<UnparsedCommand>(skip));
        }

        /// <summary>
        /// Discard the processed input.
        /// </summary>
        private void Reset()
        {
            while (i > 0)
			{
				atoms.RemoveAt(0);
				--i;
			}
            i = 0;
            count = 0;
            State = ParserExpectation.Any;
        }
    }
}

