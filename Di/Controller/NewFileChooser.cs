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
using Karl;
namespace Di.Controller
{
    public class NewFileChooser : Chooser<string>
    {
        private Model.Meta.Directory dir;
        private string name = "";

        public Event1<Model.Meta.File> ChooseFile = new Event1<Model.Meta.File>();

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
                    Candidates.Add(string.Format("[new] {0}{1}{2}", dir.ProjectRelativeFullName(), Karl.Fs.Directory.SeparatorChar, name));
                }
            }
        }

        public NewFileChooser(Main ctl, Model.Meta.Directory _dir) : base(ctl, "Name the new file", true)
        {
            dir = _dir;
            Choose.Add(EventPriority.ControllerHigh, ignore =>
            {
                var info = Karl.Fs.File.Get(dir.FullName + Karl.Fs.Directory.SeparatorChar + name);
                if (info.Exists)
                {
                    Choose.Cancel();
                    return;
                }
                info.CreateText().Close();
                try
                {
					ChooseFile.Handler(ctl.Model.Files.Get(info));
                }
                catch (FileNotIncluded)
                {
                    Choose.Cancel();
                    info.Delete();
                }
            });
        }

        public override string CandidateToString(string cand)
        {
            return cand;
        }
    }
}

