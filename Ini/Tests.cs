//  
//  Tests.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2010 Karl Voelker
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
using System.IO;
using NUnit.Framework;
namespace Ini
{
    [TestFixture]
    public class Tests
    {
        private IIniFile ini;

        [SetUp]
        public void SetUp()
        {
            ini = null;
        }

        private IList<Exception> Parse(string text)
        {
            return IniParser.Parse(new StringReader(text), "<test>", ref ini);
        }

        private void AssertHasOnlyEmptySection()
        {
            Assert.IsNotNull(ini);
            AssertContainsOnlyKeys(ini, "");
        }

        [Test]
        public void Empty()
        {
            var result = Parse("");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            AssertIsEmpty(ini[""]);
        }

        [Test]
        public void Whitespace()
        {
            var result = Parse("   \t\t\n\n \t \r\v \n  \n\t\n");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            AssertIsEmpty(ini[""]);
        }

        [Test]
        public void NoSectionsOneEntry()
        {
            var result = Parse("foo = bar");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            var section = ini[""];
            AssertContainsOnlyKeys(section, "foo");
            Assert.AreEqual(section["foo"], "bar");
        }

        [Test]
        public void NoSectionsNoEntries()
        {
            var result = Parse("foo = bar\nbaz = quux");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            var section = ini[""];
            AssertContainsOnlyKeys(section, "foo", "baz");
            Assert.AreEqual(section["foo"], "bar");
            Assert.AreEqual(section["baz"], "quux");
        }

        [Test]
        public void SomeSectionsAllEmpty()
        {
            var result = Parse("[foo] \n [bar]");
            AssertIsEmpty(result);
            Assert.IsNotNull(ini);
            AssertContainsOnlyKeys(ini, "", "foo", "bar");
            AssertIsEmpty(ini[""]);
            AssertIsEmpty(ini["foo"]);
            AssertIsEmpty(ini["bar"]);
        }

        [Test]
        public void SomeSectionsSomeEntries()
        {
            var result = Parse("baz = 5\n[foo]\n glork=spork\n [bar]\t\n\n   quux= duux");
            AssertIsEmpty(result);
            Assert.IsNotNull(ini);
            AssertContainsOnlyKeys(ini, "", "foo", "bar");
            AssertContainsOnlyKeys(ini[""], "baz");
            AssertContainsOnlyKeys(ini["foo"], "glork");
            AssertContainsOnlyKeys(ini["bar"], "quux");
            Assert.AreEqual(ini[""]["baz"], "5");
            Assert.AreEqual(ini["foo"]["glork"], "spork");
            Assert.AreEqual(ini["bar"]["quux"], "duux");
        }

        [Test]
        public void SectionWithWhitespace()
        {
            var result = Parse("[foo bar]");
            AssertIsEmpty(result);
            Assert.IsNotNull(ini);
            AssertContainsOnlyKeys(ini, "", "foo bar");
            AssertIsEmpty(ini[""]);
            AssertIsEmpty(ini["foo bar"]);
        }

        [Test]
        public void EntryWithWhitespace()
        {
            var result = Parse("foo bar= baz quux");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            AssertContainsOnlyKeys(ini[""], "foo bar");
            Assert.AreEqual(ini[""]["foo bar"], "baz quux");
        }

        [Test]
        public void EmptyValues()
        {
            var result = Parse("foo\nbar =");
            AssertIsEmpty(result);
            AssertHasOnlyEmptySection();
            AssertContainsOnlyKeys(ini[""], "foo", "bar");
            Assert.AreEqual(ini[""]["foo"], "");
            Assert.AreEqual(ini[""]["bar"], "");
        }

        public static void AssertIsEmpty<T>(ICollection<T> coll)
        {
            Assert.AreEqual(coll.Count, 0);
        }

        public static void AssertContainsKey<K, V>(IDictionary<K, V> dict, K key)
        {
            Assert.IsTrue(dict.ContainsKey(key));
        }

        public static void AssertContainsOnlyKeys<K, V>(IDictionary<K, V> dict, params K[] keys)
        {
            Assert.AreEqual(dict.Count, keys.Length);
            foreach (var key in keys)
            {
                AssertContainsKey(dict, key);
            }
        }
    }
}

