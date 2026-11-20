using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClosureCaptureTrap
{
    public static class PrefixFilter
    {
        // In-memory fix
        public static IEnumerable<string> FilterByPrefixes(
            IEnumerable<string> items,
            string[] prefixes)
        {
            Func<string, bool> predicate = _ => false;

            for (int i = 0; i < prefixes.Length; i++)
            {
                string currentPrefix = prefixes[i]; // capture value, not loop variable

                var previous = predicate;
                predicate = item =>
                    previous(item) || item.StartsWith(currentPrefix);
            }

            return items.Where(predicate);
        }

        // EF Core translatable expression
        public static Expression<Func<string, bool>> BuildPredicate(
            string[] prefixes)
        {
            var parameter = Expression.Parameter(typeof(string), "item");

            Expression body = Expression.Constant(false);

            foreach (var prefix in prefixes)
            {
                var startsWithCall = Expression.Call(
                    parameter,
                    nameof(string.StartsWith),
                    Type.EmptyTypes,
                    Expression.Constant(prefix));

                body = Expression.OrElse(body, startsWithCall);
            }

            return Expression.Lambda<Func<string, bool>>(body, parameter);
        }
    }
}
