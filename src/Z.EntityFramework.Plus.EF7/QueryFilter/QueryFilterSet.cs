// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A query filter database set.</summary>
    public class QueryFilterSet
    {
        /// <summary>Constructor.</summary>
        /// <param name="property">The property.</param>
        public QueryFilterSet(PropertyInfo property)
        {
            CreateFilterQueryableCompiled = new Lazy<Func<DbContext, QueryFilterSet, object, IQueryFilterQueryable>>(CompileCreateFilterQueryable);
            DbSetProperty = property;
            ElementType = property.PropertyType.GetDbSetElementType();
            GetDbSetCompiled = new Lazy<Func<DbContext, IQueryable>>(() => CompileGetDbSet(property));
#if EF5 || EF6
            UpdateInternalQueryCompiled = new Lazy<Action<DbContext, ObjectQuery>>(() => CompileUpdateInternalQuery(property));
#elif EF7
            //UpdateInternalQueryCompiled = new Lazy<Action<DbContext, object>>(() => CompileUpdateInternalQuery(property));
#endif
        }

        /// <summary>Gets or sets the compiled function to create a new filter queryable.</summary>
        /// <value>The compiled function to create a new filter queryable.</value>
        public Lazy<Func<DbContext, QueryFilterSet, object, IQueryFilterQueryable>> CreateFilterQueryableCompiled { get; set; }

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
        /// <summary>Adds an or get filter queryable.</summary>
        /// <param name="context">The context.</param>
        /// <returns>An IQueryFilterQueryable.</returns>
        public IQueryFilterQueryable AddOrGetFilterQueryable(DbContext context)
        {
            IQueryFilterQueryable filterQueryable;
            var set = GetDbSetCompiled.Value(context);

            if (!QueryFilterManager.CacheWeakFilterQueryable.TryGetValue(set, out filterQueryable))
            {
#if EF5 || EF6
                
                var objectQuery = set.GetObjectQuery(ElementType);
                filterQueryable = CreateFilterQueryableCompiled.Value(context, this, objectQuery);
                QueryFilterManager.CacheWeakFilterQueryable.Add(set, filterQueryable);
#elif EF7
                // todo: Create compiled version
                var field = set.GetType().GetField("_entityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = field.GetValue(set);

                var valueProperty = internalQuery.GetType().GetProperty("Value");
                var valueInternalQuery = valueProperty.GetValue(internalQuery);

                filterQueryable = CreateFilterQueryableCompiled.Value(context, this, valueInternalQuery);
                QueryFilterManager.CacheWeakFilterQueryable.Add(set, filterQueryable);
#endif
            }

            return filterQueryable;
        }

        /// <summary>Compile the function to create a new filter queryable.</summary>
        public Func<DbContext, QueryFilterSet, object, IQueryFilterQueryable> CompileCreateFilterQueryable()
        {
            var p1 = Expression.Parameter(typeof (DbContext));
            var p2 = Expression.Parameter(typeof (QueryFilterSet));
            var p3 = Expression.Parameter(typeof (object));

            var p3Convert = Expression.Convert(p3, typeof (IQueryable<>).MakeGenericType(ElementType));
            var contructorInfo = typeof (QueryFilterQueryable<>).MakeGenericType(ElementType).GetConstructors()[0];
            var expression = Expression.New(contructorInfo, p1, p2, p3Convert);

            return Expression.Lambda<Func<DbContext, QueryFilterSet, object, IQueryFilterQueryable>>(expression, p1, p2, p3).Compile();
        }

        /// <summary>Compile the function to retrieve the DbSet from the DbContext.</summary>
        /// <param name="property">The property.</param>
        public Func<DbContext, IQueryable> CompileGetDbSet(PropertyInfo property)
        {
            var p1 = Expression.Parameter(typeof (DbContext));

            var p1Convert = Expression.Convert(p1, property.DeclaringType);
            var expression = Expression.Property(p1Convert, property);

            return Expression.Lambda<Func<DbContext, IQueryable>>(expression, p1).Compile();
        }

#if EF5 || EF6
    /// <summary>Compile the action to update the internal query.</summary>
        public Action<DbContext, ObjectQuery> CompileUpdateInternalQuery(PropertyInfo property)
        {
            var dbQueryGenericType = typeof (DbQuery<>).MakeGenericType(ElementType);
            var internalQueryGenericType = typeof (DbContext).Assembly.GetType("System.Data.Entity.Internal.Linq.InternalQuery`1").MakeGenericType(ElementType);
            var lazyInternalContext = typeof (DbContext).Assembly.GetType("System.Data.Entity.Internal.LazyInternalContext");

            var internalQueryField = dbQueryGenericType.GetField("_internalQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalContextField = internalQueryGenericType.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQueryGenericConstructor = internalQueryGenericType.GetConstructor(new[] {lazyInternalContext, typeof (ObjectQuery)});

            if (internalQueryField == null || internalContextField == null || internalQueryGenericConstructor == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            var internalQuerySetValueMethod = internalQueryField.GetType().GetMethod("SetValue", new[] {typeof (object), typeof (object)});

            var p1 = Expression.Parameter(typeof (DbContext));
            var p2 = Expression.Parameter(typeof (ObjectQuery));

            var p1Convert = Expression.Convert(p1, property.DeclaringType);

            Expression dbQuery = Expression.Property(p1Convert, property);
            dbQuery = Expression.Convert(dbQuery, dbQueryGenericType);

            Expression expression = Expression.Field(dbQuery, internalQueryField);
            expression = Expression.Convert(expression, internalQueryGenericType);
            expression = Expression.Field(expression, internalContextField);

            expression = Expression.New(internalQueryGenericConstructor, expression, p2);
            expression = Expression.Call(Expression.Constant(internalQueryField), internalQuerySetValueMethod, dbQuery, Expression.Convert(expression, typeof (object)));

            return Expression.Lambda<Action<DbContext, ObjectQuery>>(expression, p1, p2).Compile();
        }
#elif EF7
        /// <summary>Compile the action to update the internal query.</summary>
        public void UpdateInternalQuery(DbContext context, object query)
        {
            // todo: Convert to expression once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            var set = GetDbSetCompiled.Value(context);

            var field = set.GetType().GetField("_entityQueryable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQuery = field.GetValue(set);

            var valueProperty = internalQuery.GetType().GetProperty("Value");


            valueProperty.SetValue(internalQuery, query);
        }
#endif
    }
}