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
namespace Di.Model
{
    public interface ICommandAtomSeq
    {
        IEnumerable<CommandAtom> Atoms { get; }
    }

    public class CommandAtom : ICommandAtomSeq
    {
        public static readonly CommandAtom
            Ignore = new CommandAtom(),
            InsertKey = new CommandAtom(),
            InsertMode = new CommandAtom(),
            CommandMode = new CommandAtom();

        private IEnumerable<CommandAtom> _atoms = null;

        public IEnumerable<CommandAtom> Atoms
        {
            get
            {
                if (_atoms == null)
                {
                    _atoms = new CommandAtom[] { this };
                }
                return _atoms;
            }
        }

        private CommandAtom()
        {
        }
    }

    public class CommandAtomSeq : ICommandAtomSeq
    {
        public IEnumerable<CommandAtom> Atoms
        {
            get;
            private set;
        }

        public CommandAtomSeq(params CommandAtom[] atoms)
        {
            Atoms = atoms;
        }
    }
}

