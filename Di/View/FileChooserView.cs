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
namespace Di.View
{
    public class FileChooserView : Gtk.VBox
    {
        private Controller.FileChooser ctl;

        public FileChooserView(Controller.FileChooser _ctl)
        {
            ctl = _ctl;
            ctl.Files.Event.Changed += Update;
            // Add a text entry box for the query
            // Add a key listener to the text entry box for Enter that calls ctl.Choose
            // Add an HBox where the query results will be listed
            // Add a label where the number of excess results will be shown
            Update();
        }

        private void Update(IList<Di.Model.ProjectFile> files)
        {
            // Clear the results box
            // For the first ten results:
            // Add one label per file to the results box, including the position of the file in the list
            // If there are more results, update and show the special label; otherwise, hide it
        }
    }
}

