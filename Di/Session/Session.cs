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
using Karl;

namespace Di.Session
{
    [Serializable]
    public class Main
    {
        [NonSerialized]
        private FileInfo file;

        private int gtkWidth;

        private int gtkHeight;

        private IList<Buffer> buffers;

        private IList<Window> windows;

        private Main(Controller.Main ctl, View.Main view)
        {
            var model = ctl.Model;
            buffers = model.Buffers.Select(mBuf => new Buffer(mBuf)).ToList();
            windows = new List<Controller.Window>(ctl.Windows).Select(cWin => new Window(FindBuffer(cWin), cWin)).ToList();
            int w, h;
            view.GetSize(out w, out h);
            gtkWidth = w;
            gtkHeight = h;
            AddHandlers(ctl, view);
        }

        private Buffer FindBuffer(Controller.Window win)
        {
            return buffers.Where(b => b.buf == win.Model.Value).First();
        }

        private void Restore(Controller.Main ctl, View.Main view)
        {
            var model = ctl.Model;
            view.WidthRequest = gtkWidth;
            view.HeightRequest = gtkHeight;
            buffers = buffers.Where(b =>
            {
                try
                {
                    b.Restore(model);
                }
                catch (CannotRestore)
                {
                    return false;
                }
                return true;
            }).ToList();
            windows = windows.Where(w =>
            {
                try
                {
                    w.Restore(model, ctl);
                }
                catch (CannotRestore)
                {
                    return false;
                }
                return true;
            }).ToList();
            AddHandlers(ctl, view);
        }

        private void AddHandlers(Controller.Main ctl, View.Main view)
        {
            var model = ctl.Model;
            model.Buffers.Added.Add((n, mBuf) => buffers.Add(new Buffer(mBuf)));
            model.Buffers.Removed.Add((n, mBuf) => buffers.Where(b => b.buf == mBuf).ToList().ForEach(b => { buffers.Remove(b); }));
            model.Buffers.Cleared.Add(buffers.Clear);
            ctl.Windows.Added.Add((n, cWin) => windows.Add(new Window(FindBuffer(cWin), cWin)));
            ctl.Windows.Removed.Add((n, cWin) => windows.Where(win => win.win == cWin).ToList().ForEach(win => { windows.Remove(win); }));
            ctl.Windows.Cleared.Add(windows.Clear);
            view.SizeAllocated += delegate(object o, Gtk.SizeAllocatedArgs args)
            {
                gtkWidth = args.Allocation.Width;
                gtkHeight = args.Allocation.Height;
            };
            ctl.Saving.Add(Save);
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
            return session;
        }

        private void Save()
        {
            buffers.ForEach(b => b.Poll());
            using (var output = file.OpenWrite())
            {
                new BinaryFormatter().Serialize(output, this);
            }
        }
    }
}

