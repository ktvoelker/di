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

        private Gtk.HBox windowsBox;

        public Main(Controller.Main c) : base(Gtk.WindowType.Toplevel)
        {
            ctl = c;
            Name = "di";
            Title = "di";
            DefaultWidth = 800;
            DefaultHeight = 600;
            DeleteEvent += OnDeleteEvent;
            windowsBox = new Gtk.HBox();
            var windowViews = new List<WindowView>();
            foreach (var window in ctl.Windows)
            {
                var view = new WindowView(window);
                windowsBox.Add(view);
                windowViews.Add(view);
            }
            Add(windowsBox);
            ctl.WindowsEvents.Added += (list, index, window) => { windowsBox.Add(new WindowView(window)); };
            ctl.WindowsEvents.Removed += (list, index, window) =>
            {
                windowsBox.Remove(windowViews[index]);
                windowViews.RemoveAt(index);
            };
            ctl.WindowsEvents.Cleared += list =>
            {
                foreach (var view in windowViews)
                {
                    windowsBox.Remove(view);
                }
                windowViews.Clear();
            };
            ctl.FocusedWindow.Changed += window =>
            {
                foreach (var view in windowViews)
                {
                    if (view.Window == window)
                    {
                        view.GrabFocus();
                    }
                }
            };
        }

        protected void OnDeleteEvent(object sender, Gtk.DeleteEventArgs a)
        {
            Gtk.Application.Quit();
            a.RetVal = true;
        }
    }
}

