using System;
using System.Collections.Generic;

namespace Jammo.ParserTools.Tools
{
    public class EnumerableIterator<T>
    {
        private readonly IEnumerable<T> enumerable;
        private readonly IEnumerator<T> enumerator;
        
        private T previous;

        public bool Started { get; private set; }
        public bool AtEnd { get; private set; }

        public T Current => enumerator.Current;

        public EnumerableIterator(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            enumerator = enumerable.GetEnumerator();
        }

        public IEnumerable<T> Enumerate()
        {
            while (TryMoveNext(out var next))
                yield return next;
        }

        public bool TryMoveNext(out T next)
        {
            next = default;

            if (Started)
                previous = enumerator.Current;
            
            if (enumerator.MoveNext())
            {
                Started = true;
                next = enumerator.Current;
                
                return true;
            }
            else
            {
                Started = true;
                AtEnd = true;

                return false;
            }
        }

        public bool TryGetPrevious(out T result)
        {
            result = previous;

            return previous == null;
        }

        public void Skip(int count = 1)
        {
            for (var c = 0; c < count; c++)
            {
                if (!TryMoveNext(out _))
                    break;
            }
        }

        public void SkipWhile(Func<T, bool> predicate)
        {
            while (TryMoveNext(out var item))
            {
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

        public IEnumerable<T> TakeWhile(Func<T, bool> predicate)
        {
            while (TryMoveNext(out var next))
            {
                if (!predicate.Invoke(next))
                    break;

                yield return next;
            }
        }

        public void Reset()
        {
            Started = false;
            enumerator.Reset();
        }
    }
}