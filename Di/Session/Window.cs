using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Di.Session
{
    [Serializable]
    class Window
    {
        [NonSerialized]
        public Controller.Window win;

        public Buffer buf;

        public int visibleOffset;

        public int cursorOffset;

        public Window(Buffer _buf, Controller.Window _ctl)
        {
            buf = _buf;
            win = _ctl;
            // TODO get visible offset
            cursorOffset = _ctl.Model.Value.GetCursorIter().GtkIter.Offset;
            AddHandlers();
        }

        public void Restore(Model.Main model, Controller.Main ctl)
        {
            if (buf.buf == null)
            {
                throw new CannotRestore();
            }
            var cWin = ctl.FindOrCreateWindow(
                new Model.File(model, new FileInfo(model.Root.FullName.AppendFsPath(buf.projectRelativeFileName))));
            win = cWin;
            cWin.Model.Value.PlaceCursor(cWin.Model.Value.GetIterAtOffset(cursorOffset));
            // TODO set visible offset
            AddHandlers();
        }

        public void AddHandlers()
        {
            win.Model.Value.MarkSet += (o, a) =>
            {
                cursorOffset = win.Model.Value.GetCursorIter().GtkIter.Offset;
            };
            // TODO add handler to watch visible offset
        }
    }
}
