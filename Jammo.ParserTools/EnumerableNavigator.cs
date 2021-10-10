using System;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools
{
    public class EnumerableNavigator<T>
    {
        private readonly T[] array;
        private int index;

        public T Current => array[index];
        public bool AtEnd => index >= array.Length - 1;

        public EnumerableNavigator(IEnumerable<T> enumerable)
        {
            array = enumerable.ToArray();
        }
        
        public void Reset()
        {
            index = 0;
        }

        public void Skip(int count = 1)
        {
            index = Math.Clamp(index + count, 0, array.Length - 1);
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

            result = array[index + 1];
            
            return true;
        }

        public bool TryMoveNext(out T result)
        {
            if (TryPeekNext(out result))
            {
                index++;
                
                return true;
            }

            return false;
        }

        public bool TryPeekLast(out T result)
        {
            result = default;
            
            if (index == 0)
                return false;
            
            result = array[index - 1];
            return true;
        }

        public bool TryMoveLast(out T result)
        {
            if (TryPeekLast(out result))
            {
                index--;

                return true;
            }

            return false;
        }
    }
}