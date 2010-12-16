//  
//  Project.cs
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
using System.IO;
namespace Di.Model
{
    public class Project
    {
        public const string ConfigFileName = "di-project.ini";

        private DirectoryInfo dir = null;

        public Project() : this(new DirectoryInfo(Environment.CurrentDirectory)) { }

        public Project(DirectoryInfo _dir)
        {
            while (!DirIsProjectRoot(_dir))
            {
                _dir = _dir.Parent;
                if (_dir == null)
                {
                    // TODO
                    // Create an exception class to throw here.
                    // It'll get caught somewhere and result in a special interaction with the user
                    // to determine which directory to create the project file in.
                    throw new InvalidOperationException();
                }
            }
            dir = _dir;
        }

        public static bool DirIsProjectRoot(DirectoryInfo dir)
        {
            return dir.GetFiles().Filter(file => file.Name == ConfigFileName).HasAny();
        }
    }
}

