using System.Collections.Generic;
using Jammo.ParserTools.Tools;

namespace Jammo.ParserTools
{
    public static class EnumerableExtensions
    {
        public static EnumerableNavigator<T> ToNavigator<T>(this IEnumerable<T> enumerable)
        {
            return new EnumerableNavigator<T>(enumerable);
        }

        public static EnumerableIterator<T> ToIterator<T>(this IEnumerable<T> enumerable)
        {
            return new EnumerableIterator<T>(enumerable);
        }
    }
}