//  
//  Chooser.cs
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
using Karl;
namespace Di.Controller
{
    public abstract class Chooser<T> : Task
    {
        public readonly string Message;

        public readonly BindList<T> Candidates;

        public readonly Event1<T> Choose = new Event1<T>();

        public readonly Event0 Cancel = new Event0();

        public abstract string Query
        {
            get;
            set;
        }

        public Chooser(Main ctl, string _message, bool allowEscape)
        {
            Message = _message;
            Candidates = new BindList<T>();
            Choose.Add(EventPriority.ControllerLow, choice => End.Handler());
            if (allowEscape)
            {
                Cancel.Add(() => End.Handler());
            }
            else
            {
                Cancel.Add(EventPriority.Max, ctl.Quit);
            }
        }

        public abstract string CandidateToString(T candidate);

    }
}

