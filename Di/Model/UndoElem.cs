//  
//  UndoElem.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2011 Karl Voelker
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
    public class UndoElem : TextStackElem<UndoElem, Buffer>
    {
        public enum ActionType
        {
            Add,
            Remove
        };

        private string text;

        private CharIter position;

        private ActionType actionType;

        public override int Size
        {
            get
            {
                return text.Length;
            }
        }

        public UndoElem(string _text, CharIter _position, ActionType _actionType)
        {
            text = _text;
            position = _position;
            actionType = _actionType;
        }

        public override void Apply(Buffer param)
        {
            switch (actionType)
            {
                case ActionType.Add:
                    param.Delete(new Range(position, text.Length));
                    break;
                case ActionType.Remove:
                    param.Insert(position, text);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override UndoElem SwapWithNeighborAbove(UndoElem other)
        {
            // Scenario 1: other (e) follows this (d)
            // abc --> abcd (add d 3) --> abcde (add e 4)
            // abc --> abce (add e 3) --> abcde (add d 3)
            //
            // Scenario 2: other (d) precedes this (e)
            // abc --> abce (add e 3) --> abcde (add d 3)
            // abc --> abcd (add d 3) --> abcde (add e 4)
            //
            // Scenario 3: other (c) in midst of this (bd)
            // a --> abd (add bd 1) --> abcd (add c 2)
            // a --> ac (add c 1) --> abcd (add b 1; add d 3)
            //
            // Scenario 4: other (bd) surrounding this (c)
            // Not possible because the "surrounding" element
            // would actually have to be two elements,
            // both of which are individually covered by the
            // preceding cases.
            throw new NotImplementedException();
        }

        public UndoElem Invert()
        {
            return new UndoElem(
                text,
                position,
                actionType == UndoElem.ActionType.Add ? UndoElem.ActionType.Remove : UndoElem.ActionType.Add);
        }
    }
}

