//  
//  ProjectFile.cs
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
    public class File : IFsQueryable
    {
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

        public FileInfo Info
        {
            get;
            private set;
        }

        public Language.Base Lang
        {
            get;
            private set;
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

        public File(Main root, FileInfo file)
        {
            Root = root;
            Info = file;
            Parent = Directory.Get(root, file.Directory);
            Lang = new Language.Plain();
        }
    }
}

