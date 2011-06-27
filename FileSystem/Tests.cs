using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FileSystem
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
            File.CacheHits = 0;
            File.CacheMisses = 0;
            rootDir = ConcatPaths(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            dirA = ConcatPaths(rootDir, "a");
            dirB = ConcatPaths(rootDir, "b");
            fileA = ConcatPaths(dirA, "a.txt");
            fileB = ConcatPaths(dirB, "b.txt");
        }

        [Test]
        public void SameFile()
        {
            var a1 = RegularFile.Get(fileA);
            var a2 = RegularFile.Get(fileA);
            Assert.AreEqual(File.CacheHits, 1);
            Assert.AreEqual(File.CacheMisses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentFiles()
        {
            var a = RegularFile.Get(fileA);
            var b = RegularFile.Get(fileB);
            Assert.AreEqual(File.CacheHits, 0);
            Assert.AreEqual(File.CacheMisses, 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void SameDirectory()
        {
            var a1 = Directory.Get(dirA);
            var a2 = Directory.Get(dirA);
            Assert.AreEqual(File.CacheHits, 1);
            Assert.AreEqual(File.CacheMisses, 1);
            Assert.AreEqual(a1, a2);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void DifferentDirectories()
        {
            var a = Directory.Get(dirA);
            var b = Directory.Get(dirB);
            Assert.AreEqual(File.CacheHits, 0);
            Assert.AreEqual(File.CacheMisses, 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void FileAsDirectory()
        {
            var a = Directory.Get(dirA);
            Assert.Throws<ArgumentException>(() => RegularFile.Get(dirA));
        }

        [Test]
        public void DirectoryAsFile()
        {
            var a = RegularFile.Get(fileA);
            Assert.Throws<ArgumentException>(() => Directory.Get(fileA));
        }
    }
}
