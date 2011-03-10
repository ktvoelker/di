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
namespace Di.Model
{
    public class Main
    {
        private readonly BindList<Buffer> buffers;
        public readonly ReadOnlyCollection<Buffer> Buffers;

        public Project CurrentProject { get; private set; }

        public Main(DirectoryInfo root)
        {
            CurrentProject = new Project(root);

            buffers = new BindList<Buffer>();
            buffers.Add(new Buffer());
            Buffers = new ReadOnlyCollection<Buffer>(buffers);
        }

        public Buffer CreateBuffer()
        {
            var buffer = new Buffer();
            buffers.Add(buffer);
            return buffer;
        }

        private Buffer CreateBuffer(ProjectFile file)
        {
            var buffer = new Buffer(file);
            buffers.Add(buffer);
            return buffer;
        }

        public Buffer FindOrCreateBuffer(ProjectFile file)
        {
            foreach (var buffer in Buffers)
            {
                if (buffer.File == file)
                {
                    return buffer;
                }
            }
            return CreateBuffer(file);
        }
    }
}

