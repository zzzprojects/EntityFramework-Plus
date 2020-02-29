#if EF6
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public class QueryCacheInterceptor : IDbTransactionInterceptor
    {
        public void ConnectionGetting(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
        }

        public void ConnectionGot(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
        }

        public void IsolationLevelGetting(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {
        }

        public void IsolationLevelGot(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {
        }

        public void Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        public void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            var context = interceptionContext.DbContexts.FirstOrDefault();

            if (context != null)
            {
                var sets = context.GetObjectContext().ObjectStateManager
                    .GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified)
                    .Select(x => x.EntitySet).Distinct().ToList();

                sets.ForEach(x => QueryCacheManager.ExpireTag(QueryCacheManager.PrefixTagSet + x.Name));
            }
        }

        public void Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        public void Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        public void RollingBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        public void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }
    }
}
#endif