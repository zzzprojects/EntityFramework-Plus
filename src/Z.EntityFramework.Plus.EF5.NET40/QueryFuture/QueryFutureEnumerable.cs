// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A future query enumerable.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class QueryFutureEnumerable<T> : QueryFutureBase, IEnumerable<T>
    {
        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">The query.</param>
        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, ObjectQuery<T> query)
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets or sets the result.</summary>
        /// <value>The result.</value>
        protected IEnumerable<T> Result { get; set; }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (!HasValue)
            {
                OwnerBatch.ExecuteQueries();
            }

            return Result.GetEnumerator();
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Sets a result.</summary>
        /// <param name="reader">The reader.</param>
        internal override void SetResult(DbDataReader reader)
        {
            // REFLECTION: Query.QueryState
            var queryStateProperty = Query.GetType().GetProperty("QueryState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var queryState = queryStateProperty.GetValue(Query, null);

            // REFLECTION: Query.QueryState.GetExecutionPlan(null)
            var getExecutionPlanMethod = queryState.GetType().GetMethod("GetExecutionPlan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getExecutionPlan = getExecutionPlanMethod.Invoke(queryState, new object[] {null});

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory
            var resultShaperFactoryField = getExecutionPlan.GetType().GetField("ResultShaperFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var resultShaperFactory = resultShaperFactoryField.GetValue(getExecutionPlan);

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters)
            var createMethod = resultShaperFactory.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if EF5
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false});
#elif EF6
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false, true});
#endif

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters).GetEnumerator()
            var getEnumeratorMethod = create.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getEnumerator = getEnumeratorMethod.Invoke(create, Type.EmptyTypes);

            var enumerator = (IEnumerator<T>) getEnumerator;
            var list = new List<T>();

            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            HasValue = true;
            Result = list;
        }
    }
}