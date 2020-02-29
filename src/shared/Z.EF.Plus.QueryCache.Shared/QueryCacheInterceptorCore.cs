//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//#if EFCORE_3X
//using System.Data.Common;
//using Microsoft.EntityFrameworkCore.Diagnostics;

//namespace Z.EntityFramework.Plus
//{
//    public class QueryCacheInterceptor : IDbTransactionInterceptor
//    {
//        public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
//        {
//            var context = eventData.Context;

//            if (context != null)
//            {
//                var sets = context.ChangeTracker
//                    .Entries()
//                    .Where(x => x.State == EntityState.Added || x.State == EntityState.Deleted || x.State == EntityState.Modified)
//                    .Select(x => x.Metadata).ToList();
//                // need a better way then meta data but still maybe do the job 
//                //.Select(x => x.).Distinct().ToList();

//                //sets.ForEach(x => QueryCacheManager.ExpireTag(QueryCacheManager.PrefixTagSet + x.Name));
//            }
//        }

//        public Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
//        {
//            //var context = eventData.Context;

//            //if (context != null)
//            //{
//            //    var sets = context.GetObjectContext().ObjectStateManager
//            //        .GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified)
//            //        .Select(x => x.EntitySet).Distinct().ToList();

//            //    sets.ForEach(x => QueryCacheManager.ExpireTag(QueryCacheManager.PrefixTagSet + x.Name));
//            //}

//            return Task.CompletedTask;
//        }

//        public InterceptionResult TransactionCommitting(DbTransaction transaction, TransactionEventData eventData, InterceptionResult result)
//        {
//            return result;
//        }

//        public Task<InterceptionResult> TransactionCommittingAsync(DbTransaction transaction, TransactionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(result);
//        }

//        public void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
//        {
//        }

//        public Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken = default)
//        {
//            return Task.CompletedTask;
//        }

//        public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
//        {
//        }

//        public Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
//        {
//            return Task.CompletedTask;
//        }

//        public InterceptionResult TransactionRollingBack(DbTransaction transaction, TransactionEventData eventData, InterceptionResult result)
//        {
//            return result;
//        }

//        public Task<InterceptionResult> TransactionRollingBackAsync(DbTransaction transaction, TransactionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(result);
//        }

//        public DbTransaction TransactionStarted(DbConnection connection, TransactionEndEventData eventData, DbTransaction result)
//        {
//            return result;
//        }

//        public Task<DbTransaction> TransactionStartedAsync(DbConnection connection, TransactionEndEventData eventData, DbTransaction result, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(result);
//        }

//        public InterceptionResult<DbTransaction> TransactionStarting(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result)
//        {
//            return result;
//        }

//        public Task<InterceptionResult<DbTransaction>> TransactionStartingAsync(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(result);
//        }

//        public DbTransaction TransactionUsed(DbConnection connection, TransactionEventData eventData, DbTransaction result)
//        {
//            return result;
//        }

//        public Task<DbTransaction> TransactionUsedAsync(DbConnection connection, TransactionEventData eventData, DbTransaction result, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(result);
//        }
//    }
//}
//#endif