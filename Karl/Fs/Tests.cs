using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Karl.Fs
{
    [TestFixture]
    class Tests
    {
        private WeakEqCache<string, Entry> cache;

        private string rootDir, dirA, dirB, fileA, fileB;

        private string ConcatPaths(string a, string b)
        {
            return a + System.IO.Path.DirectorySeparatorChar + b;
        }

        [SetUp]
        public void SetUp()
        {
            cache = Entry.Cache;
            cache.Clear();
            rootDir = ConcatPaths(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            dirA = ConcatPaths(rootDir, "a");
            dirB = ConcatPaths(rootDir, "b");
            fileA = ConcatPaths(dirA, "a.txt");
            fileB = ConcatPaths(dirB, "b.txt");
        }

        [Test]
        public void SameFile()
        {
            var a1 = File.Get(fileA);
            var a2 = File.Get(fileA);
            Assert.AreEqual(cache.Hits, 1);
            Assert.AreEqual(cache.Misses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentFiles()
        {
            var a = File.Get(fileA);
            var b = File.Get(fileB);
            Assert.AreEqual(cache.Hits, 0);
            Assert.AreEqual(cache.Misses, 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void SameDirectory()
        {
            var a1 = Directory.Get(dirA);
            var a2 = Directory.Get(dirA);
            Assert.AreEqual(cache.Hits, 1);
            Assert.AreEqual(cache.Misses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentDirectories()
        {
            var a = Directory.Get(dirA);
            var b = Directory.Get(dirB);
            Assert.AreEqual(cache.Hits, 0);
            Assert.AreEqual(cache.Misses, 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void FileAsDirectory()
        {
            var a = Directory.Get(dirA);
            Assert.Throws<ArgumentException>(() => File.Get(dirA));
        }

        [Test]
        public void DirectoryAsFile()
        {
            var a = File.Get(fileA);
            Assert.Throws<ArgumentException>(() => Directory.Get(fileA));
        }
    }
}
