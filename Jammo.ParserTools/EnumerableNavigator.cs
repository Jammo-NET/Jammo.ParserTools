using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools
{
    public class EnumerableNavigator<T>
    {
        private readonly T[] array;
        private int index;

        public T Current => array[index];
        public bool AtEnd => index < array.Length;

        public EnumerableNavigator(IEnumerable<T> enumerable)
        {
            array = enumerable.ToArray();
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