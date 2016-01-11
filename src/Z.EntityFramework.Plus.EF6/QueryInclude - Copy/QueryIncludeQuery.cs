using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    public class QueryIncludeQuery<TSource, T> : IQueryIncludeQuery
    {
        private ObjectQuery Query;
        public Expression<Func<TSource, IEnumerable<T>>> Selector;
        private IQueryable<T> IncludeQuery;
        public ObjectQuery GetObjectQuery(object orignalQuery)
        {
            var many = (orignalQuery as IQueryable<TSource>).SelectMany(Selector);
            var childs = IncludeQuery.Intersect(many);

            return childs.GetObjectQuery();
        }
        public QueryIncludeQuery(Expression<Func<TSource, IEnumerable<T>>> selector, IQueryable<T> includeQuery)
        {
            Selector = selector;
            IncludeQuery = includeQuery;
        }

        internal void SetResult(ObjectQuery Query, DbDataReader reader)
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
        }
    }
}