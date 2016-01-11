#if EF5 && NET45
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    public static partial class EF5Extensions
    {
        public static async Task<int> SaveChangesAsync(this DbContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.Run(() => context.SaveChanges(), cancellationToken).ConfigureAwait(false);
        }
    }
}

#endif