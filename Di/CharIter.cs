//  
//  CharIter.cs
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
using Gtk;
namespace Di
{
    public struct CharIter
    {
        private TextIter _iter;

        public CharIter LineStart
        {
            get
            {
                return this - _iter.LineOffset;
            }
        }

        public CharIter LineEnd
        {
            get
            {
                var ret = this;
                ret._iter.ForwardToLineEnd();
                return ret;
            }
        }

        public Gtk.TextIter GtkIter
        {
            get
            {
                return _iter;
            }
        }

        public string Char
        {
            get
            {
                return _iter.Char;
            }
        }

        public CharIter(TextIter i)
        {
            _iter = i;
        }

        public override bool Equals(object obj)
        {
            if (obj is CharIter)
            {
                return this == (CharIter) obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _iter.GetHashCode();
        }

        private static T OffsetOp<T>(CharIter i, CharIter j, Func<int, int, T> op)
        {
            return op(i._iter.Offset, j._iter.Offset);
        }

        public CharIter ForwardLines(int n)
        {
            var ret = this;
            ret._iter.ForwardLines(n);
            return ret;
        }

        public CharIter BackwardLines(int n)
        {
            var ret = this;
            ret._iter.BackwardLines(n);
            return ret;
        }

        public static CharIter operator +(CharIter i, int n)
        {
            i._iter.ForwardChars(n);
            return i;
        }

        public static CharIter operator -(CharIter i, int n)
        {
            i._iter.BackwardChars(n);
            return i;
        }

        public static int operator -(CharIter i, CharIter j)
        {
            return OffsetOp(i, j, (m, n) => m - n);
        }

        public static bool operator <(CharIter i, CharIter j)
        {
            return OffsetOp(i, j, (m, n) => m < n);
        }

        public static bool operator >(CharIter i, CharIter j)
        {
            return OffsetOp(i, j, (m, n) => m > n);
        }

        public static bool operator ==(CharIter i, CharIter j)
        {
            return OffsetOp(i, j, (m, n) => m == n);
        }

        public static bool operator !=(CharIter i, CharIter j)
        {
            return OffsetOp(i, j, (m, n) => m != n);
        }

        public static CharIter operator ++(CharIter i)
        {
            i._iter.ForwardChar();
            return i;
        }

        public static CharIter operator --(CharIter i)
        {
            i._iter.BackwardChar();
            return i;
        }
    }
}

