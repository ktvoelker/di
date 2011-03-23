//  
//  NewFileChooser.cs
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
using System.IO;
namespace Di.Controller
{
    public class NewFileChooser : Chooser<string>
    {
        private Model.Directory dir;
        private string name = "";

        public Event1<Model.File> ChooseFile = new Event1<Model.File>();

        public override string Query
        {
            get
            {
                return name;
            }

            set
            {
                name = value.Trim();
                Candidates.Clear();
                if (name != string.Empty)
                {
                    Candidates.Add(string.Format("[new] {0}{1}{2}", dir.ProjectRelativeFullName(), Path.DirectorySeparatorChar, name));
                }
            }
        }

        public NewFileChooser(Model.Directory _dir) : base("Name the new file")
        {
            dir = _dir;
            Choose.Add(ignore =>
            {
                var info = new FileInfo(dir.FullName + Path.DirectorySeparatorChar + name);
                info.CreateText().Close();
                ChooseFile.Handler(new Model.File(dir.Root, info));
            });
        }

        public override string CandidateToString(string cand)
        {
            return cand;
        }
    }
}

