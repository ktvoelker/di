using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karl
{
    public class PriorityQueue<T, P> : IEnumerable<T>
    {
        private LinkedList<Tuple<T, int>> list = new LinkedList<Tuple<T, int>>();

        private Func<P, int> proj;

        public PriorityQueue(Func<P, int> _proj)
        {
            proj = _proj;
        }

        public void Enqueue(T elem, P p)
        {
            int prio = proj(p);
            var cur = list.First;
            while (cur != null && cur.Value.Item2.CompareTo(prio) == -1)
            {
                cur = cur.Next;
            }
            if (cur == null)
            {
                list.AddLast(new Tuple<T, int>(elem, prio));
            }
            else
            {
                list.AddBefore(cur, new Tuple<T, int>(elem, prio));
            }
        }

        public T Dequeue()
        {
            var ret = list.First.Value.Item1;
            list.RemoveFirst();
            return ret;
        }

        public void Remove(T elem)
        {
            var cur = list.First;
            while (cur != null)
            {
                if (cur.Value.Item1.Equals(elem))
                {
                    list.Remove(cur);
                    return;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var elem in list)
            {
                yield return elem.Item1;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
