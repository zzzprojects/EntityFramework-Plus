#if ((EF6 && !EF5) && (NET45 || NETSTANDARD)) || EFCORE_3X
using System;
using System.Data.Common;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static class InterceptorCommandExecutingExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that will add a hook and let you make an action on the command (change command text for example) before it get executed.
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="this">The IQueryable&lt;T&gt;.</param>
        /// <param name="action">The action to execute before the command is executed.</param>
        /// <returns>The IQueryable&lt;T&gt;.</returns>
        public static IQueryable<T> InterceptorCommandExecuting<T>(this IQueryable<T> @this, Action<DbCommand> action)
        {
            return Z.EntityFramework.Plus.PublicMethodForEFPlus.InterceptorCommandExecuting(@this, action);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that will replace all "LEFT JOIN" in the query by "INNER JOIN".
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="this">The IQueryable&lt;T&gt;.</param>
        /// <returns>The IQueryable&lt;T&gt;.</returns>
        public static IQueryable<T> ReplaceAllLeftJoinByInnerJoin<T>(this IQueryable<T> @this)
        {
            return Z.EntityFramework.Plus.PublicMethodForEFPlus.ReplaceAllLeftJoinByInnerJoin(@this);
        }
    }
}
#endif