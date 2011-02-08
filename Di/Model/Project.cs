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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
namespace Di.Model
{
    public class Project
    {
        public const string ConfigFileName = "di-project.ini";

        private DirectoryInfo dir = null;
        private Ini.IIniFile config = null;

        public string Name
        {
            get
            {
                return config[""].GetWithDefault("name", "Unnamed Project");
            }
        }

        private IList<FileInfo> files;

        public ReadOnlyCollection<FileInfo> Files { get; private set; }

        public Project() : this(new DirectoryInfo(Environment.CurrentDirectory))
        {
        }

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
            Ini.IniParser.Parse(Path.Combine(dir.FullName, ConfigFileName), ref config);
            var m = new FileMatcher();
            m.ExcludeExecutableFiles = config[""].GetBoolWithDefault("exclude-exec", true);
            if (config.ContainsKey("include"))
            {
                foreach (var i in config["include"].Keys)
                {
                    m.IncludeGlob(i);
                }
            }
            if (config.ContainsKey("exclude"))
            {
                foreach (var e in config["exclude"].Keys)
                {
                    m.ExcludeGlob(e);
                }
            }
            files = m.MatchAll(dir);
            Files = new ReadOnlyCollection<FileInfo>(files);
        }

        public static bool DirIsProjectRoot(DirectoryInfo dir)
        {
            return dir.GetFiles().Where(file => file.Name == ConfigFileName).HasAny();
        }
    }
}

