using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl.Fs
{
    public class File : Entry
    {
        private System.IO.FileInfo info;

        private File(string absolutePath) : base(absolutePath)
        {
            info = new System.IO.FileInfo(AbsolutePath);
        }

        public static File Get(string path)
        {
            return Get<File>(path, p => new File(p));
        }

        public System.IO.FileStream Open(System.IO.FileMode mode)
        {
            return info.Open(mode);
        }

        public System.IO.FileStream Open(System.IO.FileMode mode, System.IO.FileAccess access)
        {
            return info.Open(mode, access);
        }

        public System.IO.StreamReader OpenText()
        {
            return info.OpenText();   
        }

        public System.IO.FileStream Create()
        {
            return info.Create();
        }

        public System.IO.StreamWriter CreateText()
        {
            return info.CreateText();
        }

        public void Delete()
        {
            info.Delete();
        }
    }
}
