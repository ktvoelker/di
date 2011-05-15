//  
//  TextStack.cs
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
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Di.Model
{
    public abstract class TextStackElem<Elem, Param> where Elem : TextStackElem<Elem, Param>
    {
        public abstract int Size
        {
            get;
        }

        public abstract void Apply(Param param);

        /// <summary>
        /// Given an element which is currently above this one on the stack, change the values of both
        /// this element and the other so that this should be above the other on the stack.
        /// </summary>
        /// <param name="other">
        /// The other element
        /// </param>
        /// <returns>
        /// A new element which should be inserted between this and the other on the stack, or null.
        /// </returns>
        public abstract Elem SwapWithNeighborAbove(Elem other);
    }

    public interface ITextStack<Elem, Param> where Elem : TextStackElem<Elem, Param>
    {
        void PopAndApply(Param p);

        void PopAndDiscard();

        void Push(Elem elem);
    }

    [Serializable]
    public class TextStack<Elem, Param> : ITextStack<Elem, Param> where Elem : TextStackElem<Elem, Param>
    {
        public const int MaxSize = 128 * 1024;

        private readonly BindList<Elem> stack = new BindList<Elem>();

        public readonly Event1<Elem> Applied = new Event1<Elem>();

        private int size = 0;

        public TextStack()
        {
        }

        public void PopAndApply(Param p)
        {
            var v = stack.Last();
            v.Apply(p);
            PopAndDiscard();
            Applied.Handler(v);
        }

        public void PopAndDiscard()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        public void Push(Elem elem)
        {
            if (MaxSize >= elem.Size)
            {
                size += elem.Size;
                stack.Add(elem);
                while (size > MaxSize)
                {
                    size -= stack[0].Size;
                    stack.RemoveAt(0);
                }
            }
        }

        public TextStackView<Elem, Param> Filter(Func<Elem, bool> pred)
        {
            Func<BindListPointer<Elem>, bool> pointerPred = p => pred(p.Value);
            return new TextStackView<Elem, Param>(stack.Add,
                () => stack.Pointers().Where(pointerPred).Select(p => new ViewElem<Elem, Param>(Applied, stack, p.AsSingleton())));
        }
    }
}

