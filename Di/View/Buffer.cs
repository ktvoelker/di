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
using Gtk;
using Pango;
namespace Di.View
{
    public class Buffer : VBox
    {
        private const uint StatusbarMode = 1;

        private Controller.Buffer ctl;
        private Gtk.ScrolledWindow scroll;
        private Gtk.Statusbar status;

        public Buffer(Controller.Buffer _ctl)
        {
            ctl = _ctl;
            Homogeneous = false;
            Spacing = 0;
            BorderWidth = 0;
            var textView = new BufferTextView(ctl);
            scroll = new Gtk.ScrolledWindow()
            {
                HscrollbarPolicy = Gtk.PolicyType.Never,
                VscrollbarPolicy = Gtk.PolicyType.Automatic
            };
            scroll.Add(textView);
            System.Action showCursor = delegate {
                var cursor = ctl.GtkTextBuffer.GetCursorIter();
                textView.ScrollToIter(cursor.GtkIter, 0, false, 0, 0);
            };
            ctl.GtkTextBuffer.MarkSet += delegate {
                showCursor();
            };
            ctl.GtkTextBuffer.Changed += delegate {
                showCursor();
            };
            PackStart(scroll, true, true, 0);
            status = new Gtk.Statusbar();
            status.Push(StatusbarMode, ctl.CurrentMode.Value.Name);
            ctl.CurrentMode.Changed += (b, m) => {
                status.Pop(StatusbarMode);
                status.Push(StatusbarMode, m.Name);
            };
            PackStart(status, false, false, 0);
        }

        private class BufferTextView : TextView
        {
            private Controller.Buffer ctl;

            public BufferTextView(Controller.Buffer _ctl) : base(_ctl.GtkTextBuffer)
            {
                ctl = _ctl;
                WrapMode = WrapMode.WordChar;
                ModifyFont(new FontDescription {
                    Family = "monospace",
                    Size = (int) (14 * Pango.Scale.PangoScale)
                });
            }

            protected override bool OnKeyPressEvent(Gdk.EventKey e)
            {
                ctl.KeyPressedHandler(e);
                return true;
            }
        }
    }
}

