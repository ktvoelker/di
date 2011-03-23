//  
//  FsChooser.cs
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
    public class FsChooser<T> : Chooser<T> where T : Model.IFsQueryable
    {
        private Func<IEnumerable<T>> getCandidates;

        private Di.Model.FsQuery<T> query;

        private string queryString;

        public override string Query
        {
            get
            {
                return queryString;
            }

            set
            {
                queryString = value;
                query = new Di.Model.FsQuery<T>(value);
                Candidates.Clear();
                Update();
            }
        }

        public FsChooser(Func<IEnumerable<T>> _getCandidates, string _message) : base(_message)
        {
            getCandidates = _getCandidates;
            query = new Di.Model.FsQuery<T>("");
            Update();
        }

        public override string CandidateToString(T candidate)
        {
            return candidate.ProjectRelativeFullName();
        }
        
        private void Update()
        {
            query.Evaluate(getCandidates()).ForEach(f => Candidates.Add(f));
        }
    }
}

