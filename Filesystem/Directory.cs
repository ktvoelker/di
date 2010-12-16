//  
//  Directory.cs
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

using IO = System.IO;

namespace Filesystem
{
    public abstract class Directory : File
    {
        public static readonly Directory Root;

        static Directory()
        {
            var drives = IO.Directory.GetLogicalDrives();
            if (drives.Length == 1)
            {
                Root = new DiskDirectory(null, drives[0]);
            }

            else
            {
                var root = new VirtualDirectory(null);
                foreach (var drive in drives)
                {
                    root.Add(drive, new DiskDirectory(root, drive));
                }
                Root = root;
            }
        }

        public static Directory Find(string path)
        {
            if (IO.Directory.Exists(path))
            {
                return new DiskDirectory(null, path);
            }
            var ex = new IO.DirectoryNotFoundException();
            ex.Data["Directory"] = path;
            throw ex;
        }

        public abstract ReadOnlyCollection<File> Files
        {
            get;
        }

        public abstract File this[string name]
        {
            get;
        }

        public abstract Directory Parent
        {
            get;
        }
    }
}

