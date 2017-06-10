// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#if EF5
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter set.</summary>
    public class QueryFilterSet
    {
        /// <summary>Constructor.</summary>
        /// <param name="dbSetProperty">The database set property.</param>
        public QueryFilterSet(PropertyInfo dbSetProperty)
        {
            CreateFilterQueryableCompiled = new Lazy<Func<DbContext, QueryFilterSet, object, BaseQueryFilterQueryable>>(CompileCreateFilterQueryable);
            DbSetProperty = dbSetProperty;
            ElementType = dbSetProperty.PropertyType.GetDbSetElementType();
            GetDbSetCompiled = new Lazy<Func<DbContext, IQueryable>>(() => CompileGetDbSet(dbSetProperty));
#if EF5 || EF6
            UpdateInternalQueryCompiled = new Lazy<Action<DbContext, ObjectQuery>>(() => CompileUpdateInternalQuery(dbSetProperty));
#elif EFCORE
    //UpdateInternalQueryCompiled = new Lazy<Action<DbContext, object>>(() => CompileUpdateInternalQuery(property));
#endif
        }

        /// <summary>Gets or sets the compiled function to create a new filter queryable.</summary>
        /// <value>The compiled function to create a new filter queryable.</value>
        public Lazy<Func<DbContext, QueryFilterSet, object, BaseQueryFilterQueryable>> CreateFilterQueryableCompiled { get; set; }

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
#if EF5 || EF6
        public Lazy<Action<DbContext, ObjectQuery>> UpdateInternalQueryCompiled { get; set; }
#elif EFCORE
        public Lazy<Action<DbContext, object>> UpdateInternalQueryCompiled { get; set; }
#endif

        /// <summary>Gets or sets the compiled action to update the internal query.</summary>
        /// <value>The compiled action to update the internal query.</value>
        /// <summary>Adds an or get a filter queryable from the context.</summary>
        /// <param name="context">The context to add or get a filter queryable.</param>
        /// <returns>the filter queryable fromt the context.</returns>
        public BaseQueryFilterQueryable AddOrGetFilterQueryable(DbContext context)
        {
            BaseQueryFilterQueryable filterQueryable;
            var set = GetDbSetCompiled.Value(context);

            if (!QueryFilterManager.CacheWeakFilterQueryable.TryGetValue(set, out filterQueryable))
            {
#if EF5 || EF6

                var objectQuery = set.GetObjectQuery(ElementType);
                filterQueryable = CreateFilterQueryableCompiled.Value(context, this, objectQuery);
                QueryFilterManager.CacheWeakFilterQueryable.Add(set, filterQueryable);
#elif EFCORE
                // todo: Create compiled version

                if(context.IsEFCore2x())
                {
                    var type = set.GetType();
                    var field = set.GetType().GetProperty("EntityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var internalQuery = field.GetValue(set);

                    filterQueryable = CreateFilterQueryableCompiled.Value(context, this, internalQuery);
                    QueryFilterManager.CacheWeakFilterQueryable.Add(set, filterQueryable);
                }
                else
                {
                    var field = set.GetType().GetField("_entityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var internalQuery = field.GetValue(set);

                    var valueProperty = internalQuery.GetType().GetProperty("Value");
                    var valueInternalQuery = valueProperty.GetValue(internalQuery);

                    filterQueryable = CreateFilterQueryableCompiled.Value(context, this, valueInternalQuery);
                    QueryFilterManager.CacheWeakFilterQueryable.Add(set, filterQueryable);
                }
#endif
            }

            return filterQueryable;
        }

        /// <summary>Compiles the function to create a new filter queryable.</summary>
        /// <returns>The compiled the function to create a new filter queryable</returns>
        public Func<DbContext, QueryFilterSet, object, BaseQueryFilterQueryable> CompileCreateFilterQueryable()
        {
            var p1 = Expression.Parameter(typeof(DbContext));
            var p2 = Expression.Parameter(typeof(QueryFilterSet));
            var p3 = Expression.Parameter(typeof(object));

            var p3Convert = Expression.Convert(p3, typeof(IQueryable<>).MakeGenericType(ElementType));
            var contructorInfo = typeof(QueryFilterQueryable<>).MakeGenericType(ElementType).GetConstructors()[0];
            var expression = Expression.New(contructorInfo, p1, p2, p3Convert);

            return Expression.Lambda<Func<DbContext, QueryFilterSet, object, BaseQueryFilterQueryable>>(expression, p1, p2, p3).Compile();
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

#if EF5 || EF6
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

#elif EFCORE
    /// <summary>Compile the action to update the internal query.</summary>
    /// <param name="context">The context to update the query from.</param>
    /// <param name="query">The query to change the internal query.</param>
        public void UpdateInternalQuery(DbContext context, object query)
        {
            // todo: Convert to expression once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            
            if(context.IsEFCore2x())
            {
                var set = GetDbSetCompiled.Value(context);

                var field = set.GetType().GetField("_entityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(set, query);
            }
            else
            {
                var set = GetDbSetCompiled.Value(context);

                var field = set.GetType().GetField("_entityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = field.GetValue(set);

                var valueProperty = internalQuery.GetType().GetProperty("Value");


                valueProperty.SetValue(internalQuery, query);
            }
        }
#endif
    }
}
#endif