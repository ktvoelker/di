//  
//  DiskDirectory.cs
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

using IO = System.IO;

namespace Filesystem
{
    internal class DiskDirectory : Directory
    {
        private string path;

        private Directory parent;

        public override Directory Parent
        {
            get
            {
                if (parent == null)
                {
                    var parent_path = IO.Directory.GetParent(path);
                    if (parent_path != null)
                    {
                        parent = new DiskDirectory(null, parent_path.FullName);
                    }
                }
                return parent;
            }
        }

        internal DiskDirectory(Directory _parent, string _path)
        {
            parent = _parent;
            path = _path;
        }
    }
}

