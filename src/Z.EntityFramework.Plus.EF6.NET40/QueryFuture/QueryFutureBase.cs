// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Common;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A future query base.</summary>
    public class QueryFutureBase
    {
        /// <summary>Gets or sets the batch that owns this item.</summary>
        /// <value>The owner batch.</value>
        internal QueryFutureBatch OwnerBatch { get; set; }

        /// <summary>Gets or sets the query.</summary>
        /// <value>The query.</value>
        internal ObjectQuery Query { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has value.</summary>
        /// <value>true if this object has value, false if not.</value>
        public bool HasValue { get; internal set; }

        /// <summary>Sets a result.</summary>
        /// <param name="reader">The reader.</param>
        internal virtual void SetResult(DbDataReader reader)
        {
        }
    }
}