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

        public Window(Buffer _buf, Controller.Window _ctl)
        {
            buf = _buf;
            win = _ctl;
            // TODO get visible offset
            AddHandlers();
        }

        public void Restore(Model.Main model, Controller.Main ctl)
        {
            if (buf.buf == null)
            {
                throw new CannotRestore();
            }
            var cWin = ctl.FindOrCreateWindow(Model.Fs.File.Get(model, Karl.Fs.File.Get(model.Root.FullName.AppendFsPath(buf.projectRelativeFileName))));
            win = cWin;
            // TODO set visible offset
            AddHandlers();
        }

        public void AddHandlers()
        {
            // TODO add handler to watch visible offset
        }
    }
}
