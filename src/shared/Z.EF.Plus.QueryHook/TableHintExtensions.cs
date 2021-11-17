#if ((EF6 && !EF5) && (NET45 || NETSTANDARD)) || EFCORE_3X
using System;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static class TableHintExtensions
    {
        /// <summary>An IQueryable&lt;T&gt; extension method that add a table "HINT" such as "NOLOCK".</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="hint">The hint.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> WithHint<T>(this IQueryable<T> @this, string hint, params Type[] types)
        {
            return PublicMethodForEFPlus.WithHint(@this, hint, types);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that add a table "HINT" such as "NOLOCK".</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="hint">The hint.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> WithHint<T>(this IQueryable<T> @this, SqlServerTableHintFlags hint, params Type[] types)
        {
            return PublicMethodForEFPlus.WithHint(@this, hint, types);
        }
    }
}
#endif