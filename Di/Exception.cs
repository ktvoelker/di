//
//  Exception.cs
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

namespace Di
{
    public class DirectoryNotIncluded : Exception
    {
        public DirectoryNotIncluded(DirectoryInfo info) : base(string.Format("The directory `{0}' is not included by the project.", info.FullName))
        {
        }
    }

    public class FileNotIncluded : Exception
    {
        public FileNotIncluded(FileInfo info) : base(string.Format("The file `{0}' is not included by the project.", info.FullName))
        {
        }
    }
}