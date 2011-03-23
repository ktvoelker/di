//  
//  Regex.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2011 Karl Voelker
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
using System.Text;
using System.Text.RegularExpressions;
namespace Di
{
    public class FileMatcher
    {
        private string includePattern = null;

        private string excludePattern = null;

        private Regex include = null;

        private Regex exclude = null;

        private Regex Include
        {
            get
            {
                if (include == null && includePattern != null)
                {
                    include = new Regex(includePattern);
                }
                return include;
            }
        }

        private Regex Exclude
        {
            get
            {
                if (exclude == null && excludePattern != null)
                {
                    exclude = new Regex(excludePattern);
                }
                return exclude;
            }
        }

        public bool ExcludeExecutableFiles
        {
            get;
            set;
        }

        public FileMatcher()
        {
        }

        public void IncludeGlob(string glob)
        {
            include = null;
            includePattern = RegexUnion(includePattern, GlobToRegex(glob));
        }

        public void ExcludeGlob(string glob)
        {
            exclude = null;
            excludePattern = RegexUnion(excludePattern, GlobToRegex(glob));
        }

        private bool MatchDefaultInclude(string path)
        {
            return Exclude == null || !Exclude.IsMatch(path) || (Include != null && Include.IsMatch(path));
        }

        private bool MatchDefaultExclude(string path)
        {
            return Include != null && Include.IsMatch(path) && (Exclude == null || !Exclude.IsMatch(path));
        }

        private bool MatchDir(DirectoryInfo dir)
        {
            return MatchDefaultInclude(dir.FullName + Path.DirectorySeparatorChar);
        }

        private bool MatchFile(FileInfo file)
        {
            return MatchDefaultExclude(file.FullName);
        }

        public void MatchAll(DirectoryInfo root, out IList<FileInfo> fileMatches, out IList<DirectoryInfo> dirMatches)
        {
            fileMatches = new List<FileInfo>();
            dirMatches = new List<DirectoryInfo>();
            MatchAllImpl(root, ref fileMatches, ref dirMatches);
        }

        private void MatchAllImpl(DirectoryInfo dir, ref IList<FileInfo> fileMatches, ref IList<DirectoryInfo> dirMatches)
        {
            if (MatchDir(dir))
            {
                dirMatches.Add(dir);
                foreach (var subdir in dir.GetDirectories())
                {
                    MatchAllImpl(subdir, ref fileMatches, ref dirMatches);
                }
                foreach (var file in dir.GetFiles())
                {
                    if (MatchFile(file))
                    {
                        fileMatches.Add(file);
                    }
                }
            }
        }

        private static string Escape(char c)
        {
            switch (c)
            {
                case '-':  // This only needs to be escaped in character classes, but it doesn't hurt.
                case '.':
                case '$':
                case '^':
                case '{':
                case '[':
                case '(':
                case '|':
                case ')':
                case '*':
                case '+':
                case '?':
                case '\\':
                    return "\\" + c;
                default:
                    return c.ToString();
            }
        }

        private static string RegexUnion(string a, string b)
        {
            if (a == null)
            {
                return b;
            }
            else if (b == null)
            {
                return a;
            }
            else
            {
                return "(?:(?:" + a + ")|(?:" + b + "))";
            }
        }

        private static string RegexUnion(IEnumerable<string> xs)
        {
            return xs.FoldLeft1(RegexUnion);
        }

        // This converts a glob to a regex.
        //
        // The regex should be matched against the full name of the file or directory in question.
        //
        // The final path component of the glob will be matched against the final path component
        // of the name being matched against.
        //
        // The initial path component of the glob may be matched against any path component
        // of the name being matched against.
        //
        // Glob writers should add a trailing path separator when wishing to match a directory.
        //
        // When the regex is being matched against a directory, the path separator should be
        // appended to the name.
        private static string GlobToRegex(string glob)
        {
            if (glob[0] == Path.DirectorySeparatorChar)
            {
                throw new ArgumentException("A glob may not start with a path separator.");
            }
            string pathSeparator = Escape(Path.DirectorySeparatorChar);
            string notPathSeparator = "[^" + pathSeparator + "]";
            var pat = new StringBuilder();
            // The match must begin at the beginning of the string or at a path separator.
            pat.Append("(?:^|(?<=" + pathSeparator + "))");
            bool inClass = false;
            var classMembers = new HashSet<char>();
            foreach (char c in glob)
            {
                if (inClass)
                {
                    if (c == ']')
                    {
                        pat.Append('(');
                        bool first = true;
                        foreach (char m in classMembers)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                pat.Append('|');
                            }
                            if (m == Path.DirectorySeparatorChar)
                            {
                                throw new ArgumentException("A glob may not have a character class containing the path separator.");
                            }
                            pat.Append(Escape(m));
                        }
                        pat.Append(')');
                    }
                    else
                    {
                        classMembers.Add(c);
                    }
                }
                else if (c == '[')
                {
                    inClass = true;
                }
                else if (c == '*')
                {
                    pat.Append(notPathSeparator + "*");
                }
                else if (c == '?')
                {
                    pat.Append(notPathSeparator);
                }
                else
                {
                    pat.Append(Escape(c));
                }
            }
            if (inClass)
            {
                throw new ArgumentException("A glob may not contain dangling character classes.");
            }
            pat.Append('$');
            return pat.ToString();
        }
    }
}

