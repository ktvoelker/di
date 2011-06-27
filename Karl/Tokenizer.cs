//  
//  Tokenizer.cs
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
using System.Text.RegularExpressions;
namespace Karl
{
    public static class Tokenizer
    {
        private static readonly Regex Pat = new Regex(@"^(?<token>\S+)(?:\s+(?<tail>.*))?$");

        public static IEnumerable<string> Tokenize(this string text)
        {
            Match m = null;
            string tail = text.Trim();
            Func<bool> incr = () =>
            {
                m = Pat.Match(tail);
                return m.Success;
            };
            while (incr())
            {
                tail = m.Groups["tail"].Value;
                yield return m.Groups["token"].Value;
            }
        }
    }
}

