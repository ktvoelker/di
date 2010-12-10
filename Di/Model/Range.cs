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
using Gtk;
namespace Di.Model
{
    public class Range
    {
        private TextIter _start;
        private TextIter _end;

        public string Chars
        {
            get
            {
                char[] chars = new char[_end.Offset - _start.Offset];
                for (TextIter i = _start; i.Offset < _end.Offset; i.ForwardChar())
                {
                    foreach (char c in i.Char)
                    {
                        chars[i.Offset - _start.Offset] = c;
                    }
                }
                return new string(chars);
            }
        }

        public Range(TextIter i, int n)
        {
            _start = i;
            _end = i;
            _end.ForwardChars(n);
        }

        public Range(TextIter s, TextIter e)
        {
            _start = s;
            _end = e;
        }
    }
}

