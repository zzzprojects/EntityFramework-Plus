// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.
#if STANDALONE
using System.Collections;
using System.Collections.Generic;
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
    /// <typeparam name="T">The type of elements of the query.</typeparam>
    internal class QueryFutureEnumerable<T> : BaseQueryFuture, IEnumerable<T>
    {
        /// <summary>The result of the query future.</summary>
        private IEnumerable<T> _result;

        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">
        ///     The query to defer the execution and to add in the batch of future
        ///     queries.
        /// </param>
#if EF5 || EF6
        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, ObjectQuery<T> query)
#elif EF7
        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, IQueryable query)
#endif
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (!HasValue)
            {
                OwnerBatch.ExecuteQueries();
            }

            return _result.GetEnumerator();
        }


        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public override void SetResult(DbDataReader reader)
        {
            var enumerator = GetQueryEnumerator<T>(reader);

            // Enumerate on all items
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            _result = list;

            HasValue = true;
        }
    }
}
#endif