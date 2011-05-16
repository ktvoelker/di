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
    [Serializable]
    public class UndoElem : TextStackElem<UndoElem, Buffer>
    {
        [Serializable]
        public enum ActionType
        {
            Add,
            Remove
        };

        private string text;

        private int position;

        private ActionType actionType;

        public override int Size
        {
            get
            {
                return text.Length;
            }
        }

        private UndoElem(string _text, int _position, ActionType _actionType)
        {
            text = _text;
            position = _position;
            actionType = _actionType;
        }

        public UndoElem(string _text, CharIter _position, ActionType _actionType) : this(_text, _position.GtkIter.Offset, _actionType)
        {
            // empty
        }

        public override void Apply(Buffer param)
        {
            param.IgnoreChanges(() =>
            {
                var iter = param.GetIterAtOffset(position);
                switch (actionType)
                {
                    case ActionType.Add:
                        param.Delete(new Range(iter, text.Length));
                        break;
                    case ActionType.Remove:
                        param.Insert(ref iter, text);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            });
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

        /// <summary>
        /// Merge another UndoElem into this one, if possible.
        /// This method assumes that the other UndoElem is the immediate chronological successor of this one.
        /// </summary>
        /// <param name="other">
        /// The UndoElem to merge into this one.
        /// </param>
        /// <returns>
        /// True iff the merge occurred.
        /// </returns>
        public bool MergeWith(UndoElem other)
        {
            if (actionType == UndoElem.ActionType.Add && other.actionType == UndoElem.ActionType.Add)
            {
                if (other.position >= position && other.position <= position + text.Length)
                {
                    // contiguous additions
                    int diff = other.position - position;
                    text = text.Substring(0, diff) + other.text + text.Substring(diff);
                    return true;
                }
                else
                {
                    // disjoint additions
                    return false;
                }
            }
            else if (actionType == UndoElem.ActionType.Remove && other.actionType == UndoElem.ActionType.Remove)
            {
                if (position >= other.position && position <= other.position + other.text.Length)
                {
                    // contiguous removals
                    int diff = position - other.position;
                    text = other.text.Substring(0, diff) + text + other.text.Substring(diff);
                    position = other.position;
                    return true;
                }
                else
                {
                    // disjoint removals
                    return false;
                }
            }
            else if (actionType == UndoElem.ActionType.Add && other.actionType == UndoElem.ActionType.Remove)
            {
                if (other.position >= position && other.position + other.text.Length <= position + text.Length)
                {
                    // removal of a subsequence of an addition
                    int diff = other.position - position;
                    text = text.Substring(0, diff) + text.Substring(diff + other.text.Length);
                    return true;
                }
                else
                {
                    // removal outside the addition
                    return false;
                }
            }
            else if (actionType == UndoElem.ActionType.Remove && other.actionType == UndoElem.ActionType.Add)
            {
                if (other.position == position && other.text.Length <= text.Length && other.text == text.Substring(0, other.text.Length))
                {
                    // addition of a prefix of a removal
                    text = text.Substring(other.text.Length);
                    position += other.text.Length;
                    return true;
                }
                else if (other.position + other.text.Length == position + text.Length && other.position >= position
                    && other.text == text.Substring(text.Length - other.text.Length))
                {
                    // addition of a suffix of a removal
                    text = text.Substring(0, text.Length - other.text.Length);
                    return true;
                }
                else
                {
                    // addition of text other than that which was removed, or which is a proper infix of that which was removed
                    return false;
                }
            }
            // impossible
            throw new InvalidOperationException();
        }
    }
}

