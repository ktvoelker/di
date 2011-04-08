using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Di.Controller
{
    public class IdleHandler
    {
        private Main ctl;

        public DateTime LastSave;

        private readonly TimeSpan MaxSaveDelay = TimeSpan.FromSeconds(60);

        public IdleHandler(Main _ctl)
        {
            ctl = _ctl;
            LastSave = DateTime.Now;
            GLib.Idle.Add(AutoSave);
        }

        private bool AutoSave()
        {
            if (LastSave + MaxSaveDelay < DateTime.Now)
            {
                ctl.Save();
            }
            return true;
        }
    }
}
