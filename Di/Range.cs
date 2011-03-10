//  
//  Range.cs
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
using System.Text;
namespace Di
{
    public struct Range
    {
        /// <summary>
        /// An iterator pointing to the first character within the range.
        /// </summary>
        public CharIter Start;

        /// <summary>
        /// An iterator pointing one past the last character within the range.
        /// </summary>
        public CharIter End;

        public string Chars
        {
            get
            {
                var sb = new StringBuilder(End - Start);
                for (CharIter i = Start; i < End; ++i)
                {
                    sb.Append(i.Char);
                }
                return sb.ToString();
            }
        }

        public Range(CharIter i, int n) : this(i, i + n) { }

        public Range(CharIter s, CharIter e)
        {
            Start = s;
            End = e;
        }

        public bool Contains(CharIter i)
        {
            return i >= Start && i < End;
        }
    }
}

