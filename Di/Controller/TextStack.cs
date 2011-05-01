//  
//  TextStack.cs
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
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Di.Controller
{
    public interface TextStackElem<T>
    {
        int Size
        {
            get;
        }

        void Apply(T param);
    }

    [Serializable]
    public class TextStack<Elem, Param> where Elem : TextStackElem<Param>
    {
        public const int MaxSize = 128 * 1024;

        private readonly IList<Elem> stack = new List<Elem>();

        private int size = 0;

        public TextStack()
        {
        }

        public Elem Peek()
        {
            return stack.Last();
        }

        public void PopAndApply(Param p)
        {
            Peek().Apply(p);
            PopAndDiscard();
        }

        public void PopAndDiscard()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        public void Push(Elem elem)
        {
            if (MaxSize >= elem.Size)
            {
                size += elem.Size;
                stack.Add(elem);
                while (size > MaxSize)
                {
                    size -= stack[0].Size;
                    stack.RemoveAt(0);
                }
            }
        }

        private static FileInfo GetFile(DirectoryInfo dir, string key)
        {
            key = string.Format("{0}{1}{1}{1}{2}", key, Path.DirectorySeparatorChar, typeof(Elem).FullName);
            var keyArray = new byte[key.Length];
            for (int i = 0; i < key.Length; ++i)
            {
                keyArray[i] = (byte) (Char.ConvertToUtf32(key, i));
            }
            return new FileInfo(dir.FullName.AppendFsPath(string.Format("{0}{1}.bin", Platform.HiddenFilePrefix,
                    SHA1.Create().ComputeHash(keyArray).Select(b => string.Format("{0:x}", b)).FoldLeft1((a, b) => a + b))));
        }

        /// <summary>
        /// Save this stack to disk.
        /// </summary>
        /// <param name="dir">
        /// The directory in which to save the file.
        /// </param>
        /// <param name="key">
        /// A key which identifies this stack. The key must be unique among stacks with the same Elem type.
        /// </param>
        public void Save(DirectoryInfo dir, string key)
        {
            using (var file = GetFile(dir, key).OpenWrite())
            {
                new BinaryFormatter().Serialize(file, this);
            }
        }

        public static TextStack<Elem, Param> Load(DirectoryInfo dir, string key)
        {
            using (var file = GetFile(dir, key).OpenRead())
            {
                return (TextStack<Elem, Param>) (new BinaryFormatter().Deserialize(file));
            }
        }
    }
}

