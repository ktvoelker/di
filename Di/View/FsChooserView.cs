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
using System.Text;
namespace Di.View
{
    public class FsChooserView<T> : Gtk.VBox where T : Model.IFsQueryable
    {
        private const int VisibleResults = 9;

        private Controller.FsChooser<T> ctl;

        private Gtk.Entry queryBox;

        public bool QueryEntryHasFocus
        {
            get
            {
                return queryBox.HasFocus;
            }
        }

        public FsChooserView(Controller.FsChooser<T> _ctl)
        {
            ctl = _ctl;
            Homogeneous = false;
            Spacing = 0;
            var topLevelBox = new Gtk.VBox();
            topLevelBox.Homogeneous = false;
            topLevelBox.Spacing = 15;
            topLevelBox.PackStart(new Gtk.HBox(), false, false, 0);
            var message = new Gtk.Label(string.Format("{0}:", ctl.Message));
            var messageAlign = new Gtk.Alignment(0, 0, 0, 0);
            messageAlign.Add(message);
            topLevelBox.PackStart(messageAlign, false, false, 0);
            queryBox = new Gtk.Entry();
            queryBox.ModifyFont(new Font(14, FontFamily.Monospace));
            queryBox.WidthChars = 30;
            queryBox.Changed += (o, e) =>
            {
                ctl.Query = queryBox.Text;
            };
            queryBox.Shown += (o, e) =>
            {
                queryBox.GrabFocus();
            };
            topLevelBox.PackStart(queryBox, false, false, 0);
            queryBox.KeyPressEvent += OnQueryKeyPress;
            var resultBox = new Gtk.TextView();
            resultBox.ModifyFont(new Font(12, FontFamily.Monospace));
            resultBox.Editable = false;
            resultBox.CursorVisible = false;
            resultBox.WrapMode = Gtk.WrapMode.WordChar;
            var resultScroll = new Gtk.ScrolledWindow();
            resultScroll.Add(resultBox);
            topLevelBox.PackStart(resultScroll, true, true, 0);
            PackStart(topLevelBox, true, true, 0);
            var excessLabel = new Gtk.Statusbar();
            excessLabel.HasResizeGrip = false;
            PackStart(excessLabel, false, false, 0);
            ctl.Files.Event.Changed += list =>
            {
                excessLabel.Pop(0);
                if (list.Count > VisibleResults)
                {
                    excessLabel.Push(0, string.Format("and {0} more results", list.Count - VisibleResults));
                }
                var sb = new StringBuilder();
                for (int i = 0; i < VisibleResults && i < list.Count; ++i)
                {
                    sb.AppendFormat("{0}. {1}\n", i + 1, list[i].ProjectRelativeFullName());
                }
                resultBox.Buffer.Text = sb.ToString();
            };
        }

        [GLib.ConnectBefore]
        private void OnQueryKeyPress(object o, Gtk.KeyPressEventArgs e)
        {
            if (e.Event.Key == Gdk.Key.Return)
            {
                ctl.Choose(ctl.Files[0]);
            }
            else if (e.Event.Key == Gdk.Key.Escape)
            {
                ctl.Cancel();
            }
        }
    }
}

