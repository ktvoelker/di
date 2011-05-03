//  
//  TextStackView.cs
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
using System.Linq;
namespace Di.Controller
{
    public class ViewElem<T, P> where T : class, ITextStackElem<T, P>
    {
        private readonly BindList<T> stack;

        private IEnumerable<BindListPointer<T>> elems;

        public IEnumerable<T> Elems
        {
            get
            {
                foreach (var elem in elems)
                {
                    yield return elem.Value;
                }
            }
        }

        public ViewElem(BindList<T> _stack, IEnumerable<BindListPointer<T>> _elems)
        {
            stack = _stack;
            elems = _elems;
        }

        private void MoveAllToTop()
        {
            var newElems = new List<BindListPointer<T>>();
            foreach (var elem in elems.OrderByDescending(e => e.Index))
            {
                while (!elem.IsLast())
                {
                    var next = elem.Next();
                    var elemValue = elem.Value;
                    var nextValue = next.Value;
                    var index = elem.Index;
                    elemValue.SwapWithNeighborAbove(nextValue);
                    next.Remove();
                    elem.Remove();
                    stack.Insert(index, nextValue);
                    stack.Insert(index, elemValue);
                    newElems.Add(new BindListPointer<T>(stack, index));
                }
            }
            elems = newElems;
        }

        public void PopAndApply(P param)
        {
            MoveAllToTop();
            foreach (var elem in elems.OrderByDescending(e => e.Index))
            {
                elem.Value.Apply(param);
                elem.Remove();
            }
        }

        public void PopAndDiscard()
        {
            MoveAllToTop();
            foreach (var elem in elems.OrderByDescending(e => e.Index))
            {
                elem.Remove();
            }
        }
    }

    public class TextStackView<Elem, Param> : ITextStack<Elem, Param> where Elem : class, ITextStackElem<Elem, Param>
    {
        private readonly Action<Elem> push;
        private readonly Func<IEnumerable<ViewElem<Elem, Param>>> view;

        public TextStackView(Action<Elem> _push, Func<IEnumerable<ViewElem<Elem, Param>>> _view)
        {
            push = _push;
            view = _view;
        }

        public void PopAndApply(Param p)
        {
            view().Last().PopAndApply(p);
        }

        public void PopAndDiscard()
        {
            view().Last().PopAndDiscard();
        }

        public void Push(Elem elem)
        {
            push(elem);
        }
    }
}

