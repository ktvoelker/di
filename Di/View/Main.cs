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

namespace Di.View
{
    public class Main : Gtk.Window
    {
        private Controller.Main ctl;

        private Gtk.HBox topLevelBox;

        private Gtk.HBox windowsBox;

        private FileChooserView chooser;

        public Main(Controller.Main c) : base(Gtk.WindowType.Toplevel)
        {
            ctl = c;
            Name = "di";
            Title = "di";
            DefaultWidth = 800;
            DefaultHeight = 600;
            DeleteEvent += OnDeleteEvent;
            topLevelBox = new Gtk.HBox();
            topLevelBox.Homogeneous = false;
            topLevelBox.Spacing = 20;
            Add(topLevelBox);
            windowsBox = new Gtk.HBox();
            windowsBox.Homogeneous = true;
            windowsBox.Spacing = 10;
            foreach (var window in ctl.Windows)
            {
                var view = new WindowView(window);
                windowsBox.Add(view);
            }
            topLevelBox.PackStart(windowsBox, true, true, 0);
            ctl.WindowsEvents.Added += (list, index, window) =>
            {
                var view = new WindowView(window);
                windowsBox.Add(view);
                windowsBox.ShowAll();
            };
            ctl.WindowsEvents.Removed += (list, index, window) =>
            {
                windowsBox.Remove(windowsBox.Children[index]);
            };
            ctl.WindowsEvents.Cleared += list =>
            {
                foreach (var view in windowsBox.Children)
                {
                    windowsBox.Remove(view);
                }
            };
            ctl.FocusedWindow.Changed += ApplyControllerFocus;
            ctl.BeginFileChooser.Add(ch =>
            {
                chooser = new FileChooserView(ch);
                topLevelBox.PackEnd(chooser, false, false, 20);
                chooser.ShowAll();
            });
            ctl.EndFileChooser.Add(ch => RemoveFileChooser());
            ctl.CancelFileChooser.Add(RemoveFileChooser);
        }

        public void ApplyControllerFocus(Controller.Window window)
        {
            foreach (var widget in windowsBox.Children)
            {
                var view = widget as WindowView;
                if (view != null && view.Window == window)
                {
                    view.FocusTextView();
                    break;
                }
            }
        }

        private void RemoveFileChooser()
        {
            bool hadFocus = chooser.QueryEntryHasFocus;
            topLevelBox.Remove(chooser);
            if (hadFocus)
            {
                ApplyControllerFocus(ctl.FocusedWindow);
            }
        }

        protected void OnDeleteEvent(object sender, Gtk.DeleteEventArgs a)
        {
            Gtk.Application.Quit();
            a.RetVal = true;
        }
    }
}

