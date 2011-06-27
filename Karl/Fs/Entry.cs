using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Karl.Fs
{
    public abstract class Entry
    {
        private Directory parent = null;

        public Directory Parent
        {
            get
            {
                if (parent == null)
                {
                    var parentInfo = new System.IO.FileInfo(AbsolutePath).Directory;
                    if (parentInfo != null)
                    {
                        parent = Directory.Get(parentInfo.FullName);
                    }
                }
                return parent;
            }
        }

        public string AbsolutePath { get; private set; }

        public string Name { get; private set; }

        public string BaseName { get; private set; }

        public string Extension { get; private set; }

        public bool Exists
        {
            get
            {
                // TODO: does this work for directories?
                return new System.IO.FileInfo(AbsolutePath).Exists;
            }
        }

        public Directory Directory
        {
            get
            {
                return Parent;
            }
        }

        public string FullName
        {
            get
            {
                return AbsolutePath;
            }
        }

        public Entry(string absolutePath)
        {
            AbsolutePath = absolutePath;
            var info = new System.IO.FileInfo(absolutePath);
            Name = info.Name;
            Extension = info.Extension;
            BaseName = Name.Substring(0, Name.Length - Extension.Length);
        }

        private static string Canonicalize(string path)
        {
            // TODO: is this good enough?
            return new System.IO.FileInfo(path).FullName;
        }

        protected static T Get<T>(string path, Func<string, T> maker) where T : Entry
        {
            return EqCache<string, Entry>.Get<T>(Canonicalize(path), maker);
        }
    }
}
