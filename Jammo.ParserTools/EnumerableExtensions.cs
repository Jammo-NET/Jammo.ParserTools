using System.Collections.Generic;

namespace Jammo.ParserTools
{
    public static class EnumerableExtensions
    {
        public static EnumerableNavigator<T> ToNavigator<T>(this IEnumerable<T> enumerable)
        {
            return new EnumerableNavigator<T>(enumerable);
        }
    }
}