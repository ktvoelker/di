//  
//  Model.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using Karl;
using Karl.Fs;
namespace Di.Model
{
    public class Main
    {
        private const string ConfigFileName = "di-config.ini";
        private const string ProjectMetaFileName = "di-project.ini";
        private const string SessionFileName = "di-session.bin";

        /// <summary>
        /// RootInfo is used by Directory to know where the root is before the Directory
        /// object tree has been created.
        /// </summary>
        public readonly Directory RootInfo;

        public readonly Meta.Directory Root;

        public readonly StrongEqCache<File, Meta.File> Files;

        public readonly StrongEqCache<Directory, Meta.Directory> Directories;

        public readonly Ini.IIniFile Config;

        private Ini.IIniFile meta = null;

        public string Name
        {
            get { return meta[""].GetWithDefault("name", "Unnamed Project"); }
        }

        public System.IO.FileInfo SessionFile
        {
            get { return new System.IO.FileInfo(RootInfo.FullName.AppendFsPath(Platform.HiddenFilePrefix + SessionFileName)); }
        }

        public readonly Karl.Fs.Matcher Matcher;

        public readonly BindList<Buffer> Buffers;

        public Main(Directory _dir)
        {
            Files = new StrongEqCache<File,Meta.File>(f => new Meta.File(this, f));
            Directories = new StrongEqCache<Directory, Meta.Directory>(d => new Meta.Directory(this, d));

            Config = LoadConfig();
            while (!DirIsProjectRoot(_dir))
            {
                _dir = _dir.Parent;
                if (_dir == null)
                {
                    // TODO
                    // Create an exception class to throw here.
                    // It'll get caught somewhere and result in a special interaction with the user
                    // to determine which directory to create the project file in.
                    throw new InvalidOperationException();
                }
            }
            RootInfo = _dir;
            Ini.IniParser.Parse(System.IO.Path.Combine(RootInfo.FullName, ProjectMetaFileName), ref meta);
            Matcher = new Karl.Fs.Matcher();
            if (meta.ContainsKey("include"))
            {
                foreach (var i in meta["include"].Keys)
                {
                    Matcher.IncludeGlob(i);
                }
            }
            if (meta.ContainsKey("exclude"))
            {
                foreach (var e in meta["exclude"].Keys)
                {
                    Matcher.ExcludeGlob(e);
                }
            }
            IList<Karl.Fs.File> fileInfos;
            IList<Karl.Fs.Directory> dirInfos;
            Matcher.MatchAll(RootInfo, out fileInfos, out dirInfos);
            Meta.File.MatchCheckEnabled = false;
            Meta.Directory.MatchCheckEnabled = false;
            fileInfos.Select(f => Files.Get(f)).ToList();
            dirInfos.Select(d => Directories.Get(d)).ToList();
            Root = Directories.Get(RootInfo);
            Meta.File.MatchCheckEnabled = true;
            Meta.Directory.MatchCheckEnabled = true;

            Buffers = new BindList<Buffer>();
        }

        private Buffer CreateBuffer(Meta.File file, TextStack<UndoElem, Buffer> undo, TextStack<UndoElem, Buffer> redo)
        {
            var buffer = new Buffer(file, undo, redo);
            Buffers.Add(buffer);
            return buffer;
        }

        public Buffer FindOrCreateBuffer(Meta.File file)
        {
            return FindOrCreateBuffer(file, new TextStack<UndoElem, Buffer>(), new TextStack<UndoElem, Buffer>());
        }

        public Buffer FindOrCreateBuffer(Meta.File file, TextStack<UndoElem, Buffer> undo, TextStack<UndoElem, Buffer> redo)
        {
            foreach (var buffer in Buffers)
            {
                if (buffer.File == file)
                {
                    return buffer;
                }
            }
            return CreateBuffer(file, undo, redo);
        }

        public static bool DirIsProjectRoot(Directory dir)
        {
            return dir.GetFiles().Where(file => file.Name == ProjectMetaFileName).HasAny();
        }

        private Ini.IIniFile LoadConfig()
        {
            var files = new List<string>();
            if (!string.IsNullOrWhiteSpace(Platform.UserConfigDirectory))
            {
                files.Add(Platform.UserConfigDirectory.AppendFsPath(Platform.HiddenFilePrefix + ConfigFileName));
            }
            Ini.IIniFile ini = Ini.IniParser.CreateEmptyIniFile();
            foreach (var file in files)
            {
                if (new System.IO.FileInfo(file).Exists)
                {
                    Ini.IniParser.Parse(file, ref ini);
                }
            }
            return ini;
        }

        public void Save()
        {
            Buffers.ForEach(b => b.Save());
        }
    }
}

