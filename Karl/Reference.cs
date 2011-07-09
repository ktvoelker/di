using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl
{
    public interface IReference<T> where T : class
    {
        T Target
        {
            get;
        }

        bool IsAlive
        {
            get;
        }
    }

    public class WeakReference<T> : System.WeakReference, IReference<T> where T : class
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

    public class StrongReference<T> : IReference<T> where T : class
    {
        private T target;

        public T Target
        {
            get
            {
                return target;
            }
        }

        public bool IsAlive
        {
            get
            {
                return true;
            }
        }

        public StrongReference(T _target)
        {
            target = _target;
        }
    }
}
