using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystem
{
    public class RegularFile : File
    {
        private RegularFile(string absolutePath) : base(absolutePath)
        {
            // empty
        }

        public static RegularFile Get(string path)
        {
            return Get<RegularFile>(path, p => new RegularFile(p));
        }

        public System.IO.FileStream Open(System.IO.FileMode mode)
        {
            return System.IO.File.Open(AbsolutePath, mode);
        }

        public void Create()
        {
            new System.IO.FileInfo(AbsolutePath).Create();
        }
    }
}
