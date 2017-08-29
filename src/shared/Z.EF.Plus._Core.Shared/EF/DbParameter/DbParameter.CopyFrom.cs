// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED

using System;
using System.Data.Common;
using System.Reflection;

#if EF5
using System.Data.Objects;
using System.Data.SqlClient;
#elif EF6
using System.Data.SqlClient;
#elif EFCORE
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static void CopyFrom(this DbParameter @this, DbParameter from)
        {
#if NETSTANDARD
            var fullName = @this.GetType().GetTypeInfo().FullName;
#else
            var fullName = @this.GetType().FullName;
#endif
            @this.DbType = from.DbType;
            @this.Direction = from.Direction;
            @this.IsNullable = from.IsNullable;
            @this.ParameterName = from.ParameterName;
            @this.Size = from.Size;

#if !EFCORE
            if (from is SqlParameter)
            {
                var fromSql = (SqlParameter)from;
                var toSql = (SqlParameter)@this;

                toSql.SqlDbType = fromSql.SqlDbType;
                toSql.UdtTypeName = fromSql.UdtTypeName;
            }
#endif

#if EF6
            if(fullName.Contains("Oracle") && from.GetType().GetProperty("OracleDbType") != null)
            {
                  var property = from.GetType().GetProperty("OracleDbType");
                  property.SetValue(@this, property.GetValue(from, null), new object[0]);
            }
#endif

            @this.Value = from.Value ?? DBNull.Value;
        }

        public static void CopyFrom(this DbParameter @this, DbParameter from, string newParameterName)
        {
#if NETSTANDARD
            var fullName = @this.GetType().GetTypeInfo().FullName;
#else
            var fullName = @this.GetType().FullName;
#endif
            @this.DbType = from.DbType;
            @this.Direction = from.Direction;
            @this.IsNullable = from.IsNullable;
            @this.ParameterName = newParameterName;
            @this.Size = from.Size;

#if !EFCORE
            if (from is SqlParameter)
            {
                var fromSql = (SqlParameter) from;
                var toSql = (SqlParameter) @this;

                toSql.SqlDbType = fromSql.SqlDbType;
                toSql.UdtTypeName = fromSql.UdtTypeName;
            }
#endif

#if EF6
            if (fullName.Contains("Oracle") && from.GetType().GetProperty("OracleDbType") != null)
            {
                var property = from.GetType().GetProperty("OracleDbType");
                property.SetValue(@this, property.GetValue(from, null), new object[0]);
            }
#endif

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

            if (from is TypeMappedRelationalParameter)
            {
                var relationalTypeMappingProperty = typeof(TypeMappedRelationalParameter).GetProperty("RelationalTypeMapping", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (relationalTypeMappingProperty != null)
                {
                    var relationalTypeMapping = (RelationalTypeMapping)relationalTypeMappingProperty.GetValue(from);

                    if (relationalTypeMapping.DbType.HasValue)
                    {
                        @this.DbType = relationalTypeMapping.DbType.Value;
                    }
                }
            }

            @this.Value = value ?? DBNull.Value;
        }

        public static void CopyFrom(this DbParameter @this, IRelationalParameter from, object value, string newParameterName)
        {
            @this.ParameterName = newParameterName;

            if (from is TypeMappedRelationalParameter)
            {
                var relationalTypeMappingProperty = typeof(TypeMappedRelationalParameter).GetProperty("RelationalTypeMapping", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (relationalTypeMappingProperty != null)
                {
                    var relationalTypeMapping = (RelationalTypeMapping) relationalTypeMappingProperty.GetValue(from);

                    if (relationalTypeMapping.DbType.HasValue)
                    {
                        @this.DbType = relationalTypeMapping.DbType.Value;
                    }
                }
            }

            @this.Value = value ?? DBNull.Value;
        }
#endif
    }
}
#endif