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
using System.Linq;

namespace Di.Model
{
    using IEC = System.Collections.Generic.IEnumerable<ICommand>;
    using IEU = System.Collections.Generic.IEnumerable<UnparsedCommand>;

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

    public class ParseResult
    {
        public IEC Commands
        {
            get;
            private set;
        }

        public ParserExpectation State
        {
            get;
            private set;
        }

        public IEnumerable<IEU> Skipped
        {
            get;
            private set;
        }

        private IList<UnparsedCommand> atoms;
        private int i;
        private RangeCommand rangeCmd;
        private uint count;

        public ParseResult(IList<UnparsedCommand> _atoms)
        {
            atoms = _atoms;
            var commands = new List<ICommand>();
            var skipped = new List<IEU>();
            State = ParserExpectation.Any;
            i = 0;
            rangeCmd = null;
            count = 0;

            while (i < atoms.Count)
            {
                var atom = atoms[0].Atom;
                var input = atoms[0].Input;
                switch (State)
                {
                    // Initial state
                    case ParserExpectation.Any:
                        // Lone, repeat, or move command
                        var loneCmd = atom as LoneCommand;
                        if (loneCmd != null)
                        {
                            commands.Add(loneCmd);
                            ++i;
                            Reset();
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
                        TryNumCommand(atom, input);
                        break;

                    // After a range command
                    case ParserExpectation.AfterRange:
                        // Same range command
                        var sameRangeCmd = atom as RangeCommand;
                        if (sameRangeCmd == rangeCmd)
                        {

                        }
                        break;
                }
            }

            Commands = commands;
            Skipped = skipped;
        }

        private void TryNumCommand(ICommand cmd, uint input)
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
                }
                else
                {
                    Skip();
                    Reset();
                }
            }
        }

        /// <summary>
        /// Add the processed input to the list failed input sequences.
        /// </summary>
        private void Skip()
        {
            var skipped = new List<UnparsedCommand>();
            skipped.AddRange(atoms.Take(i));
        }

        /// <summary>
        /// Discard the processed input.
        /// </summary>
        private void Reset()
        {
            atoms = atoms.Skip(i).ToList();
            i = 0;
            State = ParserExpectation.Any;
        }
    }
}

