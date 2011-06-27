using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystem
{
    public class Directory : File
    {
        private Directory(string absolutePath) : base(absolutePath)
        {
            // empty
        }

        public static Directory Get(string path)
        {
            return Get<Directory>(path, p => new Directory(p));
        }

        public RegularFile GetChildFile(string name)
        {
            return RegularFile.Get(AbsolutePath + System.IO.Path.DirectorySeparatorChar + name);
        }

        public Directory GetChildDirectory(string name)
        {
            return Directory.Get(AbsolutePath + System.IO.Path.DirectorySeparatorChar + name);
        }

        public void Create()
        {
            new System.IO.DirectoryInfo(AbsolutePath).Create();
        }
    }
}
