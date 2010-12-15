//  
//  View.cs
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
using Gtk;

namespace Di.View
{
    public class Main : Window
    {
        private Controller.Main ctl;

        private HBox buffersBox;

        public Main(Controller.Main c) : base(WindowType.Toplevel)
        {
            ctl = c;
            Name = "di";
            Title = "di";
            DefaultWidth = 800;
            DefaultHeight = 600;
            DeleteEvent += OnDeleteEvent;
            buffersBox = new HBox();
            foreach (var buffer in ctl.Windows)
            {
                buffersBox.Add(new Buffer(buffer));
            }
            Add(buffersBox);
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}

