using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public class QueryIncludeProvider<T, T2> : IQueryProvider
    {
        private readonly QueryIncludeOrderedQueryable<T, T2> IncludeOrderedQuery;
        private readonly QueryIncludeQueryable<T, T2> IncludeQuery;
        public IQueryProvider OriginalProvider;

        public QueryIncludeProvider(IQueryProvider provider, QueryIncludeQueryable<T, T2> includeQuery)
        {
            OriginalProvider = provider;
            IncludeQuery = includeQuery;
        }

        public QueryIncludeProvider(IQueryProvider provider, QueryIncludeOrderedQueryable<T, T2> includeQuery)
        {
            OriginalProvider = provider;
            IncludeOrderedQuery = includeQuery;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return OriginalProvider.CreateQuery(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof (TElement) == typeof (T))
            {
                var query = (IQueryable<T>) OriginalProvider.CreateQuery<TElement>(expression);

                if (query is IOrderedQueryable<TElement>)
                {
                    var includeQuery = new QueryIncludeOrderedQueryable<T, T2>(query, IncludeQuery.Selector, IncludeQuery.IncludeQuery);
                    return (IOrderedQueryable<TElement>) includeQuery;
                }
                else
                {
                    var includeQuery = new QueryIncludeQueryable<T, T2>(query, IncludeQuery.Selector, IncludeQuery.IncludeQuery);
                    return (IQueryable<TElement>) includeQuery;
                }
            }
            throw new Exception("not supported yet");
        }

        public object Execute(Expression expression)
        {
            return OriginalProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return OriginalProvider.Execute<TResult>(expression);
        }
    }
}