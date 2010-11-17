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
namespace Di.Model
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

    public class ParseResult
    {
        public IEnumerable<CommandAtom.Command> Commands
        {
            get;
            private set;
        }

        public ParserExpectation State
        {
            get;
            private set;
        }

        public IEnumerable<UnparsedCommandAtom> UnparsedAtoms
        {
            get;
            private set;
        }

        public ParseResult(IEnumerable<CommandAtom.Command> commands, IEnumerable<UnparsedCommandAtom> unparsedAtoms)
        {
            Commands = commands;
            UnparsedAtoms = unparsedAtoms;
        }
    }

    public class CommandParser
    {
        public CommandParser()
        {
        }

        public ParseResult Parse(IEnumerable<UnparsedCommandAtom> atoms)
        {
            // TODO
            throw new FormatException();
        }
    }
}

