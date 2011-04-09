//  
//  Buffer.cs
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
using System.IO;
using Gtk;
namespace Di.Model
{
    public class Buffer : TextBuffer
    {
        private static TextTagTable tags = new TextTagTable();

        public readonly Bind<bool> HasUnsavedChanges = new Bind<bool>(false);

        public File File
        {
            get;
            private set;
        }

        public Buffer(File _file) : base(tags)
        {
            File = _file;
            var input = File.Info.OpenText();
            InsertAtCursor(input.ReadToEnd());
            input.Close();
            PlaceCursor(GetIterAtOffset(0));
            Changed += (o, a) =>
            {
                HasUnsavedChanges.Value = true;
            };
        }

        public void InsertAtCursor(char c)
        {
            InsertAtCursor(string.Format("{0}", c));
        }

        public void Save()
        {
            if (HasUnsavedChanges && File != null)
            {
                var output = new StreamWriter(File.Info.Open(FileMode.Truncate, FileAccess.Write));
                output.Write(Text);
                output.Close();
                HasUnsavedChanges.Value = false;
            }
        }
    }
}

