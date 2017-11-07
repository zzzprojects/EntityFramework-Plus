#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static bool IsInMemoryEffortQueryContext(this ObjectContext @this)
        {
            return @this.GetDbContext().IsInMemoryEffortQueryContext();
        }
    }
}

#endif
#endif