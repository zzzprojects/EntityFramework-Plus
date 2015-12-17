// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Reflection;
using Microsoft.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public static class ObjectContextExtensions
    {
        public static DbContext GetDbContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            var internalContext = dbSet.GetType().GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance);
            return (DbContext) internalContext.GetValue(dbSet);
        }
    }
}