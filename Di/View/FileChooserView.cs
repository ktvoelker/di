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
        private const int VisibleResults = 9;

        private Controller.FileChooser ctl;

        public FileChooserView(Controller.FileChooser _ctl)
        {
            ctl = _ctl;
            var queryBox = new Gtk.Entry();
            queryBox.Changed += (o, e) =>
            {
                ctl.Query = queryBox.Text;
            };
            queryBox.Shown += (o, e) =>
            {
                queryBox.GrabFocus();
            };
            Add(queryBox);
            queryBox.KeyPressEvent += (object o, Gtk.KeyPressEventArgs e) =>
            {
                if (e.Event.Key == Gdk.Key.Return)
                {
                    ctl.Choose(ctl.Files[0]);
                }
                else if (e.Event.Key == Gdk.Key.Escape)
                {
                    ctl.Cancel();
                }
            };
            var resultBox = new Gtk.VBox();
            Add(resultBox);
            var excessLabel = new Gtk.Label();
            excessLabel.NoShowAll = true;
            Add(excessLabel);
            ctl.Files.Event.Changed += list =>
            {
                foreach (var child in resultBox.Children)
                {
                    resultBox.Remove(child);
                }
                if (list.Count <= VisibleResults)
                {
                    excessLabel.Text = "";
                    excessLabel.NoShowAll = true;
                }
                else
                {
                    excessLabel.Text = string.Format("and {0} more results", list.Count - VisibleResults);
                    excessLabel.NoShowAll = false;
                }
                for (int i = 0; i < VisibleResults && i < list.Count; ++i)
                {
                    var label = new Gtk.Label(string.Format("{0}. {1}", i + 1, list[i].File.FullName));
                    label.LineWrap = true;
                    label.LineWrapMode = Pango.WrapMode.Char;
                    resultBox.Add(label);
                }
                resultBox.ShowAll();
            };
        }
    }
}

