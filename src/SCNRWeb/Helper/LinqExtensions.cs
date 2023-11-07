using System;
using System.Collections.Generic;

namespace Tim.Frontend.Web.Helper
{
    public static class LinqExtensions
    {
        public static TSource? AfterOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            bool found = false;

            foreach (TSource element in source)
            {
                if (found)
                    return element;

                if (predicate(element))
                    found = true;
            }

            return default;
        }
    }
}
