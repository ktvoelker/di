using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl.Fs
{
    public class Directory : Entry
    {
        public static readonly char SeparatorChar = System.IO.Path.DirectorySeparatorChar;

        private System.IO.DirectoryInfo info;

        private Directory(string absolutePath) : base(absolutePath)
        {
            info = new System.IO.DirectoryInfo(AbsolutePath);
        }

        public static Directory Get(string path)
        {
            return Get<Directory>(path, p => new Directory(p));
        }

        public File GetChildFile(string name)
        {
            return File.Get(AbsolutePath + System.IO.Path.DirectorySeparatorChar + name);
        }

        public Directory GetChildDirectory(string name)
        {
            return Directory.Get(AbsolutePath + System.IO.Path.DirectorySeparatorChar + name);
        }

        public void Create()
        {
            info.Create();
        }

        public Directory[] GetDirectories()
        {
            return info.GetDirectories().Select(i => Get(i.FullName)).ToArray();
        }

        public File[] GetFiles()
        {
            return info.GetFiles().Select(i => File.Get(i.FullName)).ToArray();
        }
    }
}
