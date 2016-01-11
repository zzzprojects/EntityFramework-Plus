// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if EF5
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