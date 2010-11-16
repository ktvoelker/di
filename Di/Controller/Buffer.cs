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
using System.Text;
using Gtk;
using Gdk;
namespace Di.Controller
{
    public class Buffer
    {
        private Model.Buffer model;
        public TextBuffer GtkTextBuffer
        {
            get { return model; }
        }

        public Buffer(Model.Buffer _model)
        {
            model = _model;
        }

        public void KeyPressedHandler(EventKey e)
        {
            char c = (char) e.KeyValue;
            if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
            {
                model.InsertAtCursor(new string(new char[] { c }));
            }
        }
    }
}

