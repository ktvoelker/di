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
    public class FsWrapper<T> where T : FileSystemInfo
    {
        private T info;

        public FsWrapper(T _info)
        {
            info = _info;
        }

        public static implicit operator T(FsWrapper<T> w)
        {
            return w.info;
        }

        public static implicit operator FsWrapper<T>(T info)
        {
            return new FsWrapper<T>(info);
        }

        public override bool Equals(object o)
        {
            var other = o as FsWrapper<T>;
            return object.ReferenceEquals(other, null) ? false : this == other;
        }

        public override int GetHashCode()
        {
            return info.FullName.GetHashCode();
        }

        public override string ToString()
        {
            return info.ToString();
        }

        public static bool operator ==(FsWrapper<T> a, FsWrapper<T> b)
        {
            return EqUtils.EqOpByProjection<FsWrapper<T>, string>(a, b, w => w.info.FullName);
        }

        public static bool operator !=(FsWrapper<T> a, FsWrapper<T> b)
        {
            return !(a == b);
        }
    }
}

