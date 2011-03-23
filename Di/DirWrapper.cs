//  
//  DirWrapper.cs
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
    /// <summary>
    /// A wrapper for a System.IO.DirectoryInfo which performs equality comparison
    /// and hashing based on the full name.
    /// </summary>
    public class DirWrapper
    {
        private DirectoryInfo dir;

        public DirWrapper(DirectoryInfo _dir)
        {
            dir = _dir;
        }

        public static implicit operator DirectoryInfo(DirWrapper w)
        {
            return w.dir;
        }

        public static implicit operator DirWrapper(DirectoryInfo dir)
        {
            return new DirWrapper(dir);
        }

        public override bool Equals(object o)
        {
            var other = o as DirWrapper;
            return object.ReferenceEquals(other, null) ? false : this == other;
        }

        public override int GetHashCode()
        {
            return dir.FullName.GetHashCode();
        }

        public override string ToString()
        {
            return dir.ToString();
        }

        public static bool operator ==(DirWrapper a, DirWrapper b)
        {
            return EqUtils.EqOpByProjection<DirWrapper, string>(a, b, w => w.dir.FullName);
        }

        public static bool operator !=(DirWrapper a, DirWrapper b)
        {
            return !(a == b);
        }
    }
}

