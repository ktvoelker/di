//  
//  IniFile.cs
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
using System.IO;

namespace Ini
{
    public class ParseException : Exception
    {
        public uint LineNumber
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public ParseException(string message) : base(message) { }
    }

    public class IniFile
    {
        private delegate void AddProblem(string message);

        public static IList<Exception> Parse(string fileName, ref IDictionary<string, IDictionary<string, string>> dict)
        {
            if (dict == null)
            {
                dict = new Dictionary<string, IDictionary<string, string>>();
            }
            var input = File.OpenText(fileName);
            string line = null;
            IDictionary<string, string> curSection = new Dictionary<string, string>();
            dict[null] = curSection;
            var problems = new List<Exception>();
            uint lineNumber = 1;
            AddProblem addProblem = message =>
            {
                problems.Add(new ParseException(message) {LineNumber = lineNumber,FileName = fileName});
            };
            while ((line = input.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("["))
                {
                    if (line.EndsWith("]"))
                    {
                        line = line.Substring(1, line.Length - 2);
                    }
                    else
                    {
                        addProblem("Missing right bracket");
                        line = line.Substring(1, line.Length - 1);
                    }
                    curSection = new Dictionary<string, string>();
                    dict[line] = curSection;
                }
                else if (line.Length > 0)
                {
                    var parts = line.Split(new char[] { '=' }, 2);
                    curSection[parts[0].Trim()] = parts.Length == 2 ? parts[1].Trim() : "";
                }
            }
            input.Close();
            return problems;
        }
    }
}

