// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter set.</summary>
    public class QueryFilterSet
    {
        public static ConcurrentDictionary<PropertyInfo, Action<DbContext, ObjectQuery>> CachedActions = new ConcurrentDictionary<PropertyInfo, Action<DbContext, ObjectQuery>>();

        /// <summary>Constructor.</summary>
        /// <param name="dbSetProperty">The database set property.</param>
        public QueryFilterSet(DbContext context, PropertyInfo dbSetProperty)
        {
            // NOT longer used?
            // CreateFilterQueryableCompiled = new Lazy<Func<DbContext, QueryFilterSet, object, IQueryable>>(CompileCreateFilterQueryable);
            DbSetProperty = dbSetProperty;
            ElementType = dbSetProperty.PropertyType.GetDbSetElementType();
            GetDbSetCompiled = new Lazy<Func<DbContext, IQueryable>>(() => CompileGetDbSet(dbSetProperty));

            // UpdateInternalQueryCompiled
            {
                Action<DbContext, ObjectQuery> compiled;
                if (!CachedActions.TryGetValue(dbSetProperty, out compiled))
                {
                    compiled = CompileUpdateInternalQuery(dbSetProperty);
                    CachedActions.TryAdd(dbSetProperty, compiled);
                }
                UpdateInternalQueryCompiled = compiled;
            }

            {
                var currentQuery = dbSetProperty.GetValue(context, null);
                currentQuery = QueryFilterManager.HookFilter2((IQueryable)currentQuery, ElementType, QueryFilterManager.PrefixFilterID);
                OriginalQuery = (IQueryable)currentQuery;
            }
        }

        public IQueryable OriginalQuery { get; set; }
        /// <summary>Gets or sets the compiled function to create a new filter queryable.</summary>
        /// <value>The compiled function to create a new filter queryable.</value>
        public Lazy<Func<DbContext, QueryFilterSet, object, IQueryable>> CreateFilterQueryableCompiled { get; set; }

        /// <summary>Gets or sets the database set property.</summary>
        /// <value>The database set property.</value>
        public PropertyInfo DbSetProperty { get; set; }

        /// <summary>Gets or sets the type of the element.</summary>
        /// <value>The type of the element.</value>
        public Type ElementType { get; set; }

        /// <summary>Gets or sets the compiled function to retrieve the DbSet from the DbContext.</summary>
        /// <value>The compiled function to retrieve the DbSet from the DbContext.</value>
        public Lazy<Func<DbContext, IQueryable>> GetDbSetCompiled { get; set; }

        /// <summary>Gets or sets the compiled action to update the internal query.</summary>
        /// <value>The compiled action to update the internal query.</value>
        public Action<DbContext, ObjectQuery> UpdateInternalQueryCompiled { get; set; }

        /// <summary>Compiles the function to create a new filter queryable.</summary>
        /// <returns>The compiled the function to create a new filter queryable</returns>
        public Func<DbContext, QueryFilterSet, object, IQueryable> CompileCreateFilterQueryable()
        {
            var p1 = Expression.Parameter(typeof(DbContext));
            var p2 = Expression.Parameter(typeof(QueryFilterSet));
            var p3 = Expression.Parameter(typeof(object));

            var p3Convert = Expression.Convert(p3, typeof(IQueryable<>).MakeGenericType(ElementType));
            var contructorInfo = typeof(IQueryable<>).MakeGenericType(ElementType).GetConstructors()[0];
            var expression = Expression.New(contructorInfo, p1, p2, p3Convert);

            return Expression.Lambda<Func<DbContext, QueryFilterSet, object, IQueryable>>(expression, p1, p2, p3).Compile();
        }

        /// <summary>Compiles the function to retrieve the DbSet from the DbContext.</summary>
        /// <param name="dbSetProperty">The database set property.</param>
        /// <returns>The compiled the function to retrieve the DbSet from the DbContext.</returns>
        public Func<DbContext, IQueryable> CompileGetDbSet(PropertyInfo dbSetProperty)
        {
            if (dbSetProperty.DeclaringType == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            var p1 = Expression.Parameter(typeof(DbContext));
            var p1Convert = Expression.Convert(p1, dbSetProperty.DeclaringType);
            var expression = Expression.Property(p1Convert, dbSetProperty);

            return Expression.Lambda<Func<DbContext, IQueryable>>(expression, p1).Compile();
        }

        /// <summary>Compiles the action to update the internal query.</summary>
        /// <param name="dbSetProperty">The database set property.</param>
        /// <returns>The compiled the action to update the internal query.</returns>
        public Action<DbContext, ObjectQuery> CompileUpdateInternalQuery(PropertyInfo dbSetProperty)
        {
            var dbQueryGenericType = typeof(DbQuery<>).MakeGenericType(ElementType);
            var internalQueryGenericType = typeof(DbContext).Assembly.GetType("System.Data.Entity.Internal.Linq.InternalQuery`1").MakeGenericType(ElementType);
            var lazyInternalContext = typeof(DbContext).Assembly.GetType("System.Data.Entity.Internal.LazyInternalContext");

            var internalQueryField = dbQueryGenericType.GetField("_internalQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalContextField = internalQueryGenericType.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQueryGenericConstructor = internalQueryGenericType.GetConstructor(new[] { lazyInternalContext, typeof(ObjectQuery) });

            if (internalQueryField == null || internalContextField == null || internalQueryGenericConstructor == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            var internalQuerySetValueMethod = internalQueryField.GetType().GetMethod("SetValue", new[] { typeof(object), typeof(object) });

            var p1 = Expression.Parameter(typeof(DbContext));
            var p2 = Expression.Parameter(typeof(ObjectQuery));

            var p1Convert = Expression.Convert(p1, dbSetProperty.DeclaringType);

            Expression dbQuery = Expression.Property(p1Convert, dbSetProperty);
            dbQuery = Expression.Convert(dbQuery, dbQueryGenericType);

            Expression expression = Expression.Field(dbQuery, internalQueryField);
            expression = Expression.Convert(expression, internalQueryGenericType);
            expression = Expression.Field(expression, internalContextField);

            expression = Expression.New(internalQueryGenericConstructor, expression, p2);
            expression = Expression.Call(Expression.Constant(internalQueryField), internalQuerySetValueMethod, dbQuery, Expression.Convert(expression, typeof(object)));

            return Expression.Lambda<Action<DbContext, ObjectQuery>>(expression, p1, p2).Compile();
        }
    }
}