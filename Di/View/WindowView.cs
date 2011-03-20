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
    public class WindowView : Gtk.VBox, IContainFocus
    {
        public readonly Main View;

        private const uint StatusbarMode = 1;

        public readonly Controller.Window Window;
        private Gtk.ScrolledWindow scroll;
        private Gtk.Statusbar status;
        private WindowTextView textView;

        public Gtk.Widget FocusWidget
        {
            get
            {
                return textView;
            }
        }

        private readonly Gdk.Color HighlightColor = new Gdk.Color(0x49, 0x65, 0xD6);
        private readonly Gdk.Color NormalColor;

        public WindowView(Main _view, Controller.Window _ctl)
        {
            View = _view;
            Window = _ctl;
            Homogeneous = false;
            Spacing = 0;
            BorderWidth = 0;
            var topLevelBox = new Gtk.VBox();
            topLevelBox.Homogeneous = false;
            topLevelBox.Spacing = 0;
            topLevelBox.BorderWidth = 0;
            textView = new WindowTextView(Window);
            scroll = new Gtk.ScrolledWindow {
                HscrollbarPolicy = Gtk.PolicyType.Automatic,
                VscrollbarPolicy = Gtk.PolicyType.Automatic
            };
            scroll.Add(textView);
            Window.CursorMovedByCommand.Add(i =>
            {
                textView.ScrollToIter(i.GtkIter, 0, false, 0, 0);
            });
            topLevelBox.PackStart(scroll, true, true, 0);
            status = new Gtk.Statusbar();
            status.HasResizeGrip = false;
            status.Push(StatusbarMode, Window.CurrentMode.GetName());
            Window.CurrentMode.Event.Changed += m =>
            {
                status.Pop(StatusbarMode);
                status.Push(StatusbarMode, Window.CurrentMode.GetName());
            };
            Window.Model.Changed += m =>
            {
                textView.Buffer = m;
            };
            topLevelBox.PackStart(status, false, false, 0);

            // Wrap the topLevelBox with borders on the left and right
            var hlBox = new Gtk.DrawingArea();
            NormalColor = hlBox.Style.Background(Gtk.StateType.Normal);
            hlBox.WidthRequest = 10;
            var borderBox = new Gtk.HBox();
            borderBox.Homogeneous = false;
            borderBox.Spacing = 0;
            borderBox.BorderWidth = 0;
            borderBox.PackStart(hlBox, false, false, 0);
            borderBox.PackStart(topLevelBox, true, true, 0);

            textView.FocusInEvent += (object o, Gtk.FocusInEventArgs args) =>
            {
                Window.Controller.FocusedWindow.Value = Window;
                hlBox.ModifyBg(Gtk.StateType.Normal, HighlightColor);
            };

            textView.FocusOutEvent += (object o, Gtk.FocusOutEventArgs args) =>
            {
                hlBox.ModifyBg(Gtk.StateType.Normal, NormalColor);
            };

            Add(borderBox);
        }

        public void FocusTextView()
        {
            textView.GrabFocus();
        }

        private class WindowTextView : Gtk.TextView
        {
            private Controller.Window ctl;

            public WindowTextView(Controller.Window _ctl) : base(_ctl.Model)
            {
                ctl = _ctl;
                WrapMode = Gtk.WrapMode.WordChar;
                ModifyFont(new Font(14, FontFamily.Monospace));
            }

            protected override bool OnKeyPressEvent(Gdk.EventKey e)
            {
                ctl.KeyPressedHandler(e);
                return true;
            }
        }
    }
}

