//  
//  Session.cs
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Di.Session
{
    [Serializable]
    public class Main
    {
        [Serializable]
        private class Window
        {
            [NonSerialized]
            public Controller.Window ctl;

            public string projectRelativeFileName;

            public int visibleOffset;

            public int cursorOffset;

            public Window(Controller.Window _ctl)
            {
                ctl = _ctl;
                projectRelativeFileName = _ctl.Model.Value.File.ProjectRelativeFullName();
                // TODO get visible offset
                cursorOffset = _ctl.Model.Value.GetCursorIter().GtkIter.Offset;
                AddHandlers();
            }

            public void AddHandlers()
            {
                ctl.Model.Value.MarkSet += (o, a) =>
                {
                    cursorOffset = ctl.Model.Value.GetCursorIter().GtkIter.Offset;
                };
                // TODO add handler to watch visible offset
            }
        }

        private int gtkWidth;

        private int gtkHeight;

        private IList<Window> windows;

        [NonSerialized]
        private FileInfo file;

        private Main(Controller.Main ctl, View.Main view)
        {
            windows = new List<Controller.Window>(ctl.Windows).Select(cWin => new Window(cWin)).ToList();
            int w, h;
            view.GetSize(out w, out h);
            gtkWidth = w;
            gtkHeight = h;
        }

        private void AddHandlers(Controller.Main ctl, View.Main view)
        {
            ctl.Windows.Added.Add((n, cWin) => windows.Add(new Window(cWin)));
            ctl.Windows.Removed.Add((n, cWin) => windows.Where(win => win.ctl == cWin).ToList().ForEach(win => { windows.Remove(win); }));
            view.SizeAllocated += delegate(object o, Gtk.SizeAllocatedArgs args)
            {
                gtkWidth = args.Allocation.Width;
                gtkHeight = args.Allocation.Height;
            };
            ctl.Saving.Add(Save);
        }

        private void Restore(Controller.Main ctl, View.Main view)
        {
            var model = ctl.Model;
            view.WidthRequest = gtkWidth;
            view.HeightRequest = gtkHeight;
            foreach (var w in windows)
            {
                var cWin = ctl.FindOrCreateWindow(
                    new Model.File(model, new FileInfo(model.Root.FullName.AppendFsPath(w.projectRelativeFileName))));
                w.ctl = cWin;
                cWin.Model.Value.PlaceCursor(cWin.Model.Value.GetIterAtOffset(w.cursorOffset));
                // TODO set visible offset
                w.AddHandlers();
            }
        }

        public static Main Load(FileInfo file, Controller.Main ctl, View.Main view)
        {
            Main session;
            if (file.Exists)
            {
                using (var input = file.OpenRead())
                {
                    session = (Main) (new BinaryFormatter().Deserialize(input));
                    session.file = file;
                    session.Restore(ctl, view);
                }
            }
            else
            {
                session = new Main(ctl, view) { file = file };
            }
            session.AddHandlers(ctl, view);
            return session;
        }

        private void Save()
        {
            using (var output = file.OpenWrite())
            {
                new BinaryFormatter().Serialize(output, this);
            }
        }
    }
}

