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
        private Action<Model.File> handler;
        private string name = "";

        public override string Query
        {
            set
            {
                name = value;
                Candidates.Clear();
                Candidates.Add(string.Format("[new] {0}{1}{2}", dir.ProjectRelativeFullName(), Path.DirectorySeparatorChar, name));
            }
        }

        public NewFileChooser(Model.Directory _dir, Action<Model.File> _handler) : base("Name the new file")
        {
            dir = _dir;
            handler = _handler;
        }

        public override string CandidateToString(string cand)
        {
            return cand;
        }

        public override void Choose(string ignore)
        {
            base.Choose(ignore);
            var info = new FileInfo(dir.FullName + Path.DirectorySeparatorChar + name);
            info.CreateText().Close();
            handler(new Model.File(dir.Root, info));
        }
    }
}

