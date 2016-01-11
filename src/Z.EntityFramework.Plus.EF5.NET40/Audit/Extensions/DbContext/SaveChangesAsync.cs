// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if NET45
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    public static partial class ChangeAuditExtensions
    {
        /// <summary>Asynchronously audits and saves all changes made in this context to the underlying database.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the number of objects written
        ///     to the underlying database
        /// </returns>
        public static Task<int> SaveChangesAsync(this DbContext context, Audit audit)
        {
            return context.SaveChangesAsync(audit, CancellationToken.None);
        }

        /// <summary>A DbContext extension method that saves the changes asynchronous.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the number of objects written
        ///     to the underlying database
        /// </returns>
        public static Task<int> SaveChangesAsync(this DbContext context, Audit audit, CancellationToken cancellationToken)
        {
            AuditStateEntry.PreSaveChanges(audit, context);
            var result = context.SaveChangesAsync(cancellationToken);
            AuditStateEntry.PostSaveChanges(audit);

            if (audit.Configuration.AutoSaveAsyncAction != null)
            {
                result.ContinueWith(x =>
                {
                    audit.Configuration.AutoSaveAsyncAction(context, audit, cancellationToken);
                    return x.Result;
                }, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }
    }
}

#endif