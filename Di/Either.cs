using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Di
{
    public abstract class Either<T, U>
    {
        public V Apply<V>(Func<T, V> ifLeft, Func<U, V> ifRight)
        {
            var right = this as Right<T, U>;
            if (right == null)
            {
                return ifLeft(this.FromLeft());
            }
            else
            {
                return ifRight(right.Value);
            }
        }

        public T FromLeft()
        {
            return ((Left<T, U>)this).Value;
        }

        public U FromRight()
        {
            return ((Right<T, U>)this).Value;
        }
    }

    public class Left<T, U> : Either<T, U>
    {
        public readonly T Value;

        public Left(T value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Left<T, U> && Value.Equals(((Left<T, U>)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    public class Right<T, U> : Either<T, U>
    {
        public readonly U Value;

        public Right(U value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Right<T, U> && Value.Equals(((Right<T, U>)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
