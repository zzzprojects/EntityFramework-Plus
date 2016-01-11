// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Entity;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    public static partial class ObjectContextExtensions
    {
        public static DbContext GetDbContext<TEntity>(this IDbSet<TEntity> dbSet) where TEntity : class
        {
            var internalSet = dbSet.GetType().GetField("_internalSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dbSet);
            var internalContext = internalSet.GetType().BaseType.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(internalSet);
            return (DbContext) internalContext.GetType().GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public).GetValue(internalContext, null);
        }
    }
}