// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
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
    /// <summary>A future query value.</summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class QueryFutureValue<TResult> : QueryFutureBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">The query.</param>
        public QueryFutureValue(QueryFutureBatch ownerBatch, ObjectQuery query)
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets or sets the result.</summary>
        /// <value>The result.</value>
        protected TResult Result { get; set; }

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public TResult Value
        {
            get
            {
                if (!HasValue)
                {
                    OwnerBatch.ExecuteQueries();
                }

                return Result;
            }
        }

        /// <summary>Sets a result.</summary>
        /// <param name="reader">The reader.</param>
        internal override void SetResult(DbDataReader reader)
        {
            // Query.QueryState
            var queryStateProperty = Query.GetType().GetProperty("QueryState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var queryState = queryStateProperty.GetValue(Query, null);

            // Query.QueryState.GetExecutionPlan(null)
            var getExecutionPlanMethod = queryState.GetType().GetMethod("GetExecutionPlan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getExecutionPlan = getExecutionPlanMethod.Invoke(queryState, new object[] {null});

            // Query.QueryState.GetExecutionPlan(null).ResultShaperFactory
            var resultShaperFactoryField = getExecutionPlan.GetType().GetField("ResultShaperFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var resultShaperFactory = resultShaperFactoryField.GetValue(getExecutionPlan);

            // Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters)
            var createMethod = resultShaperFactory.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#if EF5
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false});
#elif EF6
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false, true});
#endif
            // Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters).GetEnumerator()
            var getEnumeratorMethod = create.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getEnumerator = getEnumeratorMethod.Invoke(create, Type.EmptyTypes);

            var enumerator = (IEnumerator<TResult>) getEnumerator;

            enumerator.MoveNext();

            HasValue = true;
            Result = enumerator.Current;
        }
    }
}