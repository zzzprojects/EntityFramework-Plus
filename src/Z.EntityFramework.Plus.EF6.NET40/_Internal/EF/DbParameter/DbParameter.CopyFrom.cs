// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED

using System;
using System.Data.Common;

#if EF5
using System.Data.Objects;
#elif EFCORE
using Microsoft.EntityFrameworkCore.Storage;
#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static void CopyFrom(this DbParameter @this, DbParameter from)
        {
            @this.DbType = from.DbType;
            @this.Direction = from.Direction;
            @this.IsNullable = from.IsNullable;
            @this.ParameterName = from.ParameterName;
            @this.Size = from.Size;

            @this.Value = from.Value ?? DBNull.Value;
        }

        public static void CopyFrom(this DbParameter @this, DbParameter from, string newParameterName)
        {
            @this.DbType = from.DbType;
            @this.Direction = from.Direction;
            @this.IsNullable = from.IsNullable;
            @this.ParameterName = newParameterName;
            @this.Size = from.Size;

            @this.Value = from.Value ?? DBNull.Value;
        }

#if EF5
        public static void CopyFrom(this DbParameter @this, ObjectParameter from)
        {
            @this.ParameterName = from.Name;
            
            @this.Value = from.Value ?? DBNull.Value;
        }

        public static void CopyFrom(this DbParameter @this, ObjectParameter from, string newParameterName)
        {
            @this.ParameterName = newParameterName;
            
            @this.Value = from.Value ?? DBNull.Value;
        }
#endif
#if EFCORE
        public static void CopyFrom(this DbParameter @this, IRelationalParameter from, object value)
        {
            @this.ParameterName = from.InvariantName;

            @this.Value = value ?? DBNull.Value;
        }

        public static void CopyFrom(this DbParameter @this, IRelationalParameter from, object value, string newParameterName)
        {
            @this.ParameterName = newParameterName;

            @this.Value = value ?? DBNull.Value;
        }
#endif
    }
}
#endif