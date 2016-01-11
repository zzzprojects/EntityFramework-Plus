using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>Interface for batch queryable.</summary>
    public interface IBatchQueryable
    {
        /// <summary>Gets or sets a value indicating whether this object has result.</summary>
        /// <value>true if this object has result, false if not.</value>
        bool HasResult { get; set; }

        /// <summary>Gets or sets the result.</summary>
        /// <value>The result.</value>
        object Result { get; set; }

        /// <summary>Gets or sets the batch that owns this item.</summary>
        /// <value>The owner batch.</value>
        BatchQuery OwnerBatch { get; set; }

        /// <summary>Gets or sets the parent that owns this item.</summary>
        /// <value>The owner parent.</value>
        IBatchQueryable OwnerParent { get; set; }

        /// <summary>Gets object query.</summary>
        /// <returns>The object query.</returns>
        ObjectQuery GetObjectQuery();

        /// <summary>Sets a result.</summary>
        /// <param name="reader">The reader.</param>
        void SetResult(DbDataReader reader);
        /// <summary>Creates ordered queryable.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="originalQuery">The original query.</param>
        /// <param name="updateChild">true to update child.</param>
        /// <returns>The new ordered queryable.</returns>
        BatchOrderedQueryable<TResult> CreateOrderedQueryable<TResult>(IOrderedQueryable<TResult> originalQuery, bool updateChild = true);
    }
}