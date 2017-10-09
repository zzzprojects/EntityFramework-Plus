#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF6
using System.Linq;

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
#endif