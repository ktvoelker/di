//  
//  FileChooser.cs
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
namespace Di.Controller
{
    public class FileChooser
    {
        private Di.Model.Project project;

        private Action<Di.Model.ProjectFile> handler;

        private Di.Model.FileQuery query;

        public string Query
        {
            set
            {
                query = new Di.Model.FileQuery(value);
                Files.Clear();
                Update();
            }
        }

        public BindList<Di.Model.ProjectFile> Files;

        public FileChooser(Di.Model.Project _project, Action<Di.Model.ProjectFile> _handler)
        {
            project = _project;
            handler = _handler;
            query = new Di.Model.FileQuery("");
            Files = new BindList<Di.Model.ProjectFile>();
            Update();
        }

        public void Choose(Di.Model.ProjectFile file)
        {
            handler(file);
        }

        private void Update()
        {
            query.Evaluate(project.Files).ForEach(f => Files.Add(f));
        }
    }
}

