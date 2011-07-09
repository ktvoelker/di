using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Di.Model.Meta
{
    public interface IEntry
    {
        Main Root
        {
            get;
        }

        Directory Parent
        {
            get;
        }

        Language.Base Lang
        {
            get;
        }

        string Name
        {
            get;
        }

        string FullName
        {
            get;
        }
    }

    public abstract class Entry<I> : IEntry where I : Karl.Fs.Entry
    {
        public Main Root
        {
            get;
            private set;
        }

        public readonly I Info;

        public Entry(Main root, I info)
        {
            Root = root;
            Info = info;
        }

        public Directory Parent
        {
            get;
            protected set;
        }

        public Language.Base Lang
        {
            get;
            protected set;
        }

        public string Name
        {
            get
            {
                return Info.Name;
            }
        }

        public string FullName
        {
            get
            {
                return Info.FullName;
            }
        }
    }
}
