using System.Collections.Generic;

namespace Jammo.ParserTools
{
    public class EnumerableIterator<T>
    {
        private readonly IEnumerable<T> enumerable;
        private readonly IEnumerator<T> enumerator;
        
        private T previous;
        private bool startedEnumeration;

        public bool AtEnd;

        public EnumerableIterator(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            enumerator = enumerable.GetEnumerator();
        }

        public bool TryMoveNext(out T result)
        {
            return TryGetNextInternal(out result);
        }

        public bool TryGetPrevious(out T result)
        {
            result = previous;

            return previous == null;
        }

        public void Reset()
        {
            startedEnumeration = false;
            enumerator.Reset();
        }

        private bool TryGetNextInternal(out T next)
        {
            next = default;

            if (startedEnumeration)
                previous = enumerator.Current;
            // TODO: Fix bug because enumerator works different and starts at the first item
            if (enumerator.MoveNext())
            {
                startedEnumeration = true;
                next = enumerator.Current;
                
                return true;
            }
            else
            {
                startedEnumeration = true;
                AtEnd = true;

                return false;
            }
        }
    }
}