//  
//  ProjectDirectory.cs
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
namespace Di.Model
{
    public class ProjectDirectory : IFileQueryable
    {
        private static readonly Language.Base LangInstance = new Language.Directory();

        public Project Root
        {
            get;
            private set;
        }

        public ProjectDirectory Parent
        {
            get;
            private set;
        }

        public DirectoryInfo Directory
        {
            get;
            private set;
        }

        public Language.Base Lang
        {
            get
            {
                return LangInstance;
            }
        }

        public string Name
        {
            get
            {
                return Directory.Name;
            }
        }

        public string FullName
        {
            get
            {
                return Directory.FullName;
            }
        }

        public string ProjectRelativeFullName
        {
            get { return Directory.FullName.Substring(Root.Root.FullName.Length + 1); }
        }

        public ProjectDirectory(Project root, ProjectDirectory parent, DirectoryInfo dir)
        {
            Root = root;
            Parent = parent;
            Directory = dir;
        }
    }
}

