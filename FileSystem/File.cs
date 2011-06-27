using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FileSystem
{
    public class File
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

        private static IDictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();

        private static object cacheLock = new object();

        internal static int CacheHits = 0;

        internal static int CacheMisses = 0;

        protected File(string absolutePath)
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

        protected static T Get<T>(string path, Func<string, T> maker) where T : File
        {
            string absolutePath = Canonicalize(path);
            T result = null;
            lock (cacheLock)
            {
                if (cache.ContainsKey(absolutePath))
                {
                    var thing = cache[absolutePath].Target;
                    if (thing != null)
                    {
                        result = thing as T;
                        if (result == null)
                        {
                            throw new ArgumentException("File/directory mismatch");
                        }
                        ++CacheHits;
                    }
                }
                if (result == null)
                {
                    ++CacheMisses;
                    result = maker(absolutePath);
                    cache[absolutePath] = new WeakReference(result);
                }
            }
            return result;
        }
    }
}
