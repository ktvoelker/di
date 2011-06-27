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
using System.IO;
using System.Linq;
using Karl;
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
        public readonly Karl.Fs.Directory RootInfo;

        public readonly Directory Root;

        public readonly Ini.IIniFile Config;

        private Ini.IIniFile meta = null;

        public string Name
        {
            get { return meta[""].GetWithDefault("name", "Unnamed Project"); }
        }

        public FileInfo SessionFile
        {
            get { return new FileInfo(RootInfo.FullName.AppendFsPath(Platform.HiddenFilePrefix + SessionFileName)); }
        }

        private IList<File> files;

        public ReadOnlyCollection<File> Files
        {
            get;
            private set;
        }

        private IList<Directory> directories;

        public ReadOnlyCollection<Directory> Directories
        {
            get;
            private set;
        }

        public readonly Karl.Fs.Matcher Matcher;

        public readonly BindList<Buffer> Buffers;

        public Main(Karl.Fs.Directory _dir)
        {
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
            Ini.IniParser.Parse(Path.Combine(RootInfo.FullName, ProjectMetaFileName), ref meta);
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
            File.MatchCheckEnabled = false;
            Directory.MatchCheckEnabled = false;
            files = fileInfos.Select(f => Fs.File.Get(this, f)).ToList();
            directories = dirInfos.Select(d => Fs.Directory.Get(this, d)).ToList();
            Root = Fs.Directory.Get(this, RootInfo);
            File.MatchCheckEnabled = true;
            Directory.MatchCheckEnabled = true;
            Files = new ReadOnlyCollection<File>(files);
            Directories = new ReadOnlyCollection<Directory>(directories);

            Buffers = new BindList<Buffer>();
        }

        private Buffer CreateBuffer(File file, TextStack<UndoElem, Buffer> undo, TextStack<UndoElem, Buffer> redo)
        {
            var buffer = new Buffer(file, undo, redo);
            Buffers.Add(buffer);
            return buffer;
        }

        public Buffer FindOrCreateBuffer(File file)
        {
            return FindOrCreateBuffer(file, new TextStack<UndoElem, Buffer>(), new TextStack<UndoElem, Buffer>());
        }

        public Buffer FindOrCreateBuffer(File file, TextStack<UndoElem, Buffer> undo, TextStack<UndoElem, Buffer> redo)
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

        public static bool DirIsProjectRoot(Karl.Fs.Directory dir)
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
                if (new FileInfo(file).Exists)
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

