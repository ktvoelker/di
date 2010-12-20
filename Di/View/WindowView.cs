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
using Pango;
namespace Di.View
{
    public class WindowView : Gtk.VBox
    {
        private const uint StatusbarMode = 1;

        public readonly Controller.Window Window;
        private Gtk.ScrolledWindow scroll;
        private Gtk.Statusbar status;

        public WindowView(Controller.Window _ctl)
        {
            Window = _ctl;
            Homogeneous = false;
            Spacing = 0;
            BorderWidth = 0;
            var textView = new WindowTextView(Window);
            scroll = new Gtk.ScrolledWindow { HscrollbarPolicy = Gtk.PolicyType.Never, VscrollbarPolicy = Gtk.PolicyType.Automatic };
            scroll.Add(textView);
            System.Action showCursor = delegate
            {
                var cursor = Window.GtkTextBuffer.GetCursorIter();
                textView.ScrollToIter(cursor.GtkIter, 0, false, 0, 0);
            };
            Window.GtkTextBuffer.MarkSet += delegate { showCursor(); };
            Window.GtkTextBuffer.Changed += delegate { showCursor(); };
            PackStart(scroll, true, true, 0);
            status = new Gtk.Statusbar();
            status.Push(StatusbarMode, Window.CurrentMode.GetName());
            Window.CurrentMode.Event.Changed += m =>
            {
                status.Pop(StatusbarMode);
                status.Push(StatusbarMode, Window.CurrentMode.GetName());
            };
            PackStart(status, false, false, 0);
        }

        private class WindowTextView : Gtk.TextView
        {
            private Controller.Window ctl;

            public WindowTextView(Controller.Window _ctl) : base(_ctl.GtkTextBuffer)
            {
                ctl = _ctl;
                WrapMode = Gtk.WrapMode.WordChar;
                ModifyFont(new FontDescription { Family = "monospace", Size = (int) (14 * Pango.Scale.PangoScale) });
            }

            protected override bool OnKeyPressEvent(Gdk.EventKey e)
            {
                ctl.KeyPressedHandler(e);
                return true;
            }
        }
    }
}

