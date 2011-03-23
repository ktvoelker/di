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
using System.Collections.Generic;
using System.IO;
namespace Di.Model
{
    public class Directory : IFsQueryable
    {
        private static readonly Language.Base LangInstance = new Language.Directory();

        public Main Root
        {
            get;
            private set;
        }

        public Directory Parent
        {
            get;
            private set;
        }

        public DirectoryInfo Info
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
                return Info.Name;
            }
        }

        public string FullName
        {
            get
            {
                return Info.FullName;
            }
        }

        private static IDictionary<Main, IDictionary<DirWrapper, Directory>> Directories;

        static Directory()
        {
            Directories = new Dictionary<Main, IDictionary<DirWrapper, Directory>>();
        }

        private Directory(Main root, DirectoryInfo info)
        {
            Root = root;
            Info = info;
            Parent = info.FullName == root.RootInfo.FullName ? null : Get(root, info.Parent);
        }

        public static Directory Get(Main root, DirectoryInfo info)
        {
            if (!Directories.ContainsKey(root))
            {
                Directories[root] = new Dictionary<DirWrapper, Directory>();
            }
            if (!Directories[root].ContainsKey(info))
            {
                Directories[root][info] = new Directory(root, info);
            }
            return Directories[root][info];
        }

        public static IEnumerable<Directory> GetAll(Main root)
        {
            return Directories[root].Values;
        }
    }
}

