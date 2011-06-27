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
        private string rootDir = null, dirA = null, dirB = null, fileA = null, fileB = null;

        private string ConcatPaths(string a, string b)
        {
            return a + System.IO.Path.DirectorySeparatorChar + b;
        }

        [SetUp]
        public void SetUp()
        {
            Entry.CacheHits = 0;
            Entry.CacheMisses = 0;
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
            Assert.AreEqual(Entry.CacheHits, 1);
            Assert.AreEqual(Entry.CacheMisses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentFiles()
        {
            var a = File.Get(fileA);
            var b = File.Get(fileB);
            Assert.AreEqual(Entry.CacheHits, 0);
            Assert.AreEqual(Entry.CacheMisses, 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void SameDirectory()
        {
            var a1 = Directory.Get(dirA);
            var a2 = Directory.Get(dirA);
            Assert.AreEqual(Entry.CacheHits, 1);
            Assert.AreEqual(Entry.CacheMisses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentDirectories()
        {
            var a = Directory.Get(dirA);
            var b = Directory.Get(dirB);
            Assert.AreEqual(Entry.CacheHits, 0);
            Assert.AreEqual(Entry.CacheMisses, 2);
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
