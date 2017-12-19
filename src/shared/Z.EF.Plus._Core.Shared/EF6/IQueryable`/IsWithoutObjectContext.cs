#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF6
using System.Linq;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static bool IsWithoutObjectContext<T>(this IQueryable<T> q)
        {
            var fullName = q.GetType().FullName;
            return fullName.StartsWith("System.Collections.ObjectModel.ObservableCollection");
        }
    }
}

#endif
#endif