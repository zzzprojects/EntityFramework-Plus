#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF6
using System.Data.Entity;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static bool IsInMemoryEffortQueryContext(this DbContext @this)
        {
            return @this.Database.Connection.GetType().FullName == "Effort.Provider.EffortConnection";
        }
    }
}

#endif
#endif