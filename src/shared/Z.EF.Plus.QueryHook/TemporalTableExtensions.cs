#if ((EF6 && !EF5) && (NET45 || NETSTANDARD)) || EFCORE_3X
using System;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static class TemporalTableExtensions
    {
        /// <summary>An IQueryable&lt;T&gt; extension method that allow to query a temporal table with the "AS OF&lt;date_time&gt;" expression.</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> TemporalTableAsOf<T>(this IQueryable<T> @this, DateTime dateTime, params Type[] types)
        {
            return PublicMethodForEFPlus.TemporalTableAsOf(@this, dateTime, types);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that allow to query a temporal table with the "FROM&lt;start_date_time&gt;TO&lt;end_date_time&gt;" expression.</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> TemporalTableFromTo<T>(this IQueryable<T> @this, DateTime startDateTime, DateTime endDateTime, params Type[] types)
        {
            return PublicMethodForEFPlus.TemporalTableFromTo(@this, startDateTime, endDateTime, types);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that allow to query a temporal table with the "BETWEEN&lt;start_date_time&gt;AND&lt;end_date_time&gt;" expression.</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> TemporalTableBetween<T>(this IQueryable<T> @this, DateTime startDateTime, DateTime endDateTime, params Type[] types)
        {
            return PublicMethodForEFPlus.TemporalTableBetween(@this, startDateTime, endDateTime, types);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that allow to query a temporal table with the "CONTAINED IN (&lt;start_date_time&gt; , &lt;end_date_time&gt;)" expression.</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> TemporalTableContainedIn<T>(this IQueryable<T> @this, DateTime startDateTime, DateTime endDateTime, params Type[] types)
        {
            return PublicMethodForEFPlus.TemporalTableContainedIn(@this, startDateTime, endDateTime, types);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that allow to query a temporal table with the "ALL" expression.</summary>
        /// <param name="this">The query to act on.</param>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> TemporalTableAll<T>(this IQueryable<T> @this, params Type[] types)
        {
            return PublicMethodForEFPlus.TemporalTableAll(@this, types);
        }
    }
}
#endif