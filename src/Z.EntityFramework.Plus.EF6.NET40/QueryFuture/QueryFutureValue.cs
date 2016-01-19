// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Common;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EF7
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class for query future value.</summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class QueryFutureValue<TResult> : BaseQueryFuture
    {
        /// <summary>The result of the query future.</summary>
        private TResult _result;

        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">
        ///     The query to defer the execution and to add in the batch of future
        ///     queries.
        /// </param>
#if EF5 || EF6
        public QueryFutureValue(QueryFutureBatch ownerBatch, ObjectQuery query)
#elif EF7
        public QueryFutureValue(QueryFutureBatch ownerBatch, IQueryable query)
#endif
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets the value of the future query.</summary>
        /// <value>The value of the future query.</value>
        public TResult Value
        {
            get
            {
                if (!HasValue)
                {
                    OwnerBatch.ExecuteQueries();
                }

                return _result;
            }
        }

        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public override void SetResult(DbDataReader reader)
        {
            var enumerator = GetQueryEnumerator<TResult>(reader);

            // Enumerate on first item only
            enumerator.MoveNext();
            _result = enumerator.Current;

            HasValue = true;
        }
    }
}