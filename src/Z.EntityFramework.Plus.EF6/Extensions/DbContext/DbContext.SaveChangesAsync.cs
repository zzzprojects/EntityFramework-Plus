// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.


#if EF5 && NET45
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    public static partial class Extensions
    {
        public static Task<int> SaveChangesAsync(this DbContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => context.SaveChanges(), cancellationToken);
        }
    }
}

#endif