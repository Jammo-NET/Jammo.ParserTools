using System;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools
{
    public class EnumerableNavigator<T>
    {
        private readonly T[] array;

        public int Index { get; private set; } = -1;
        public bool Started => Index > -1;

        public T Current
        {
            get
            {
                if (!Started)
                    throw new InvalidOperationException("Enumeration has not started.");

                return array[Index];
            }
        }

        public bool AtEnd => Index >= array.Length - 1;

        public EnumerableNavigator(IEnumerable<T> enumerable)
        {
            array = enumerable.ToArray();
        }
        
        public void Reset()
        {
            Index = -1;
        }

        public void Skip(int count = 1)
        {
            for (var c = 0; c <= count; c++)
            {
                if (!TryMoveNext(out _))
                    break;
            }
        }

        public void SkipWhile(Func<T, bool> predicate)
        {
            while (TryPeekNext(out var item))
            {
                TryMoveNext(out _);
                
                if (!predicate.Invoke(item))
                    break;
            }
        }
        
        public IEnumerable<T> Take(int count)
        {
            for (var c = 0; c < count; c++)
            {
                if (AtEnd)
                    break;
                
                if (!TryMoveNext(out var item))
                    break;

                yield return item;
            }
        }

        public bool TakeIf(Func<T, bool> predicate, out T result)
        {
            result = default;
            
            if (AtEnd)
                return false;
            
            TryPeekNext(out var item);

            if (predicate.Invoke(item))
            {
                TryMoveNext(out result);

                return true;
            }

            return false;
        }

        public IEnumerable<T> TakeWhile(Func<T, bool> predicate)
        {
            while (TakeIf(predicate, out var item))
                yield return item;
        }

        public IEnumerable<T> EnumerateFromIndex()
        {
            while (TryMoveNext(out var item))
                yield return item;
        }

        public IEnumerable<T> EnumerateOnce()
        {
            foreach (var item in array)
                yield return item;
        }

        public bool TryPeekNext(out T result)
        {
            result = default;
            
            if (AtEnd)
                return false;

            result = array[Index + 1];
            
            return true;
        }

        public bool TryMoveNext(out T result)
        {
            if (TryPeekNext(out result))
            {
                Index++;
                
                return true;
            }

            return false;
        }

        public bool TryPeekLast(out T result)
        {
            result = default;
            
            if (!Started || Index == 0)
                return false;
            
            result = array[Index - 1];
            return true;
        }

        public bool TryMoveLast(out T result)
        {
            if (TryPeekLast(out result))
            {
                Index--;

                return true;
            }

            return false;
        }
    }
}