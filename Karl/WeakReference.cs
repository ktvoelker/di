using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl
{
    class WeakReference<T> : System.WeakReference where T : class
    {
        public new T Target
        {
            get
            {
                return (T)(base.Target);
            }
        }

        public WeakReference(T target) : base(target)
        {
            // empty
        }
    }
}
