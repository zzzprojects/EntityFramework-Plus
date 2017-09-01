
#if EF6
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static bool IsInMemoryEffortQueryContext<T>(this IQueryable<T> q)
        {
            return q.GetDbContext().Database.Connection.GetType().FullName == "Effort.Provider.EffortConnection";
        }
    }
}

#endif
