﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl
{
    [Serializable]
    public enum RemovePolicy
    {
        PreviousBecomesCurrent,
        NextBecomesCurrent
    }

    [Serializable]
    public enum UnknownSetPolicy
    {
        AddToStart,
        AddToEnd,
        AddAfterCurrent,
        Error
    }

    [Serializable]
    public enum WrapPolicy
    {
        Wrap,
        DontWrap
    }

    /// <summary>
    /// A BindList with a current element.
    /// There is always a current element unless the list is empty.
    /// If the list is empty, CurrentIndex == -1 and Current == null.
    /// Note that Current may be null at other times if the list contains a null element.
    /// If the list is not empty, this[CurrentIndex] == Current.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BindListWithCurrent<T> : BindList<T> where T : class
    {
        [NonSerialized]
        public readonly Event2<int, T> CurrentChanged = new Event2<int, T>();

        public RemovePolicy RemovePolicy = RemovePolicy.NextBecomesCurrent;

        public UnknownSetPolicy UnknownSetPolicy = UnknownSetPolicy.AddToEnd;

        public WrapPolicy WrapPolicy = WrapPolicy.Wrap;

        private int cur;

        public int CurrentIndex
        {
            get
            {
                return cur;
            }

            set
            {
                if (value < -1 || (value == -1 && Count > 0) || value >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                cur = value;
                CurrentChanged.Handler(cur, Current);
            }
        }

        public T Current
        {
            get
            {
                return cur == -1 ? null : this[cur];
            }

            set
            {
                int found = this.IndexOf(value);
                if (found >= 0)
                {
                    CurrentIndex = found;
                }
                else
                {
                    int index;
                    switch (UnknownSetPolicy)
                    {
                        case UnknownSetPolicy.AddAfterCurrent:
                            index = CurrentIndex + 1;
                            break;
                        case UnknownSetPolicy.AddToEnd:
                            index = Count;
                            break;
                        case UnknownSetPolicy.AddToStart:
                            index = 0;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                    Insert(index, value);
                    CurrentIndex = index;
                }
            }
        }

        public BindListWithCurrent()
        {
            cur = -1;
            Cleared.Add(() =>
            {
                CurrentIndex = -1;
            });
            Added.Add((idx, elem) =>
            {
                if (cur == -1)
                {
                    CurrentIndex = idx;
                }
                else if (idx <= cur)
                {
                    CurrentIndex = cur + 1;
                }
            });
            Removed.Add((idx, elem) =>
            {
                if (idx == cur)
                {
                    if (Count == 0)
                    {
                        CurrentIndex = -1;
                    }
                    else if (RemovePolicy == RemovePolicy.PreviousBecomesCurrent)
                    {
                        Previous();
                    }
                    else
                    {
                        Next();
                    }
                }
            });
        }

        public void Previous()
        {
            if (cur == 0)
            {
                if (WrapPolicy == WrapPolicy.Wrap)
                {
                    CurrentIndex = Count - 1;
                }
            }
            else
            {
                CurrentIndex = cur - 1;
            }
        }

        public void Next()
        {
            if (cur == Count)
            {
                if (WrapPolicy == WrapPolicy.Wrap)
                {
                    CurrentIndex = 0;
                }
            }
            else
            {
                CurrentIndex = cur;
            }
        }
    }
}
