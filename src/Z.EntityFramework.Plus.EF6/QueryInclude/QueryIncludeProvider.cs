using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A batch query provider.</summary>
    public class QueryIncludeProvider<T> : IQueryProvider where T: class
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
        public QueryIncludeProvider(IQueryProvider originalProvider)
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryIncludeQueryable<T> CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
        public IQueryProvider OriginalProvider { get; set; }

        /// <summary>Creates a query.</summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The new query.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            // THIS method should never be called
            throw new Exception(ExceptionMessage.GeneralException);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var query = OriginalProvider.CreateQuery<TElement>(expression);
            return null;
            // return CurrentQueryable.CreateOrderedQueryable(query as IOrderedQueryable<TElement>);
        }

        public object Execute(Expression expression)
        {
            // THIS method should never be called
            throw new Exception(ExceptionMessage.GeneralException);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)(object)null;
            //var objectQuery = CurrentQueryable.GetObjectQuery();

            //// GET provider
            //var objectQueryProviderProperty = objectQuery.GetType().GetProperty("ObjectQueryProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            //var provider = (IQueryProvider) objectQueryProviderProperty.GetValue(objectQuery);

            //// CREATE query from the expression
            //var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (Expression)}, null);
            //createQueryMethod = createQueryMethod.MakeGenericMethod(typeof (TResult));
            //var query = createQueryMethod.Invoke(provider, new object[] {expression});

            //var orderedQuery = query as IOrderedQueryable<TResult>;
            //var queryNew = CurrentQueryable.CreateOrderedQueryable(orderedQuery, false);

            //// EXECUTE the batch
            //queryNew.OwnerBatch.Execute();

            //// SET result as single value
            //// todo: make something about first or default...
            //queryNew.Result = ((IEnumerable<TResult>) queryNew.Result).FirstOrDefault();

            //return (TResult) queryNew.Result;
        }
    }
}