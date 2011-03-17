//  
//  FsQuery.cs
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
using System.Linq;
namespace Di.Model
{
    public interface IFsQueryable
    {
        Main Root
        {
            get;
        }

        Directory Parent
        {
            get;
        }

        Language.Base Lang
        {
            get;
        }

        string Name
        {
            get;
        }

        string FullName
        {
            get;
        }
    }

    public class FsQuery<T> where T : IFsQueryable
    {
        private string query;

        public FsQuery(string _query)
        {
            query = _query;
        }

        public IQueryable<T> Evaluate(IEnumerable<T> files)
        {
            return files.AsQueryable().Where(f => f.Name == query);
        }
    }
}

