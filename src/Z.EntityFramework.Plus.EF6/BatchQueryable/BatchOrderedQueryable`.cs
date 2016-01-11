using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A batch ordered queryable.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class BatchOrderedQueryable<T> : IBatchQueryable, IOrderedQueryable<T>
    {
        Action<IEnumerable<T>> ActionTrial { get; set; }
        /// <summary>Constructor.</summary>
        /// <param name="query">The query.</param>
        public BatchOrderedQueryable(IQueryable<T> query)
        {
            Childs = new List<IBatchQueryable>();
            ObjectQuery = query.GetObjectQuery();
            OriginalQueryable = query;
        }

        public BatchOrderedQueryable(IQueryable<T> query, Action<IEnumerable<T>> actionTrial)
        {
            ActionTrial = actionTrial;
            Childs = new List<IBatchQueryable>();
            ObjectQuery = query.GetObjectQuery();
            OriginalQueryable = query;
        }

        /// <summary>Constructor.</summary>
        /// <param name="queryFactory">The query factory.</param>
        public BatchOrderedQueryable(Func<ObjectQuery, IQueryable<T>> queryFactory)
        {
            Childs = new List<IBatchQueryable>();
            QueryFactory = queryFactory;
        }

        /// <summary>Gets or sets the childs.</summary>
        /// <value>The childs.</value>
        public List<IBatchQueryable> Childs { get; set; }

        /// <summary>Gets or sets the internal provider.</summary>
        /// <value>The internal provider.</value>
        public BatchQueryProvider InternalProvider { get; set; }

        /// <summary>Gets or sets the object query.</summary>
        /// <value>The object query.</value>
        public ObjectQuery ObjectQuery { get; set; }

        /// <summary>Gets or sets the original queryable.</summary>
        /// <value>The original queryable.</value>
        public IQueryable<T> OriginalQueryable { get; set; }

        /// <summary>Gets or sets the query factory.</summary>
        /// <value>The query factory.</value>
        public Func<ObjectQuery, IQueryable<T>> QueryFactory { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has result.</summary>
        /// <value>true if this object has result, false if not.</value>
        public bool HasResult { get; set; }

        /// <summary>Gets or sets the batch that owns this item.</summary>
        /// <value>The owner batch.</value>
        public BatchQuery OwnerBatch { get; set; }

        /// <summary>Gets or sets the parent that owns this item.</summary>
        /// <value>The owner parent.</value>
        public IBatchQueryable OwnerParent { get; set; }


        /// <summary>Gets or sets the result.</summary>
        /// <value>The result.</value>
        public object Result { get; set; }


        /// <summary>Sets a result.</summary>
        /// <param name="reader">The reader.</param>
        public void SetResult(DbDataReader reader)
        {
            // REFLECTION: Query.QueryState
            var queryStateProperty = ObjectQuery.GetType().GetProperty("QueryState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var queryState = queryStateProperty.GetValue(ObjectQuery, null);

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
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, ObjectQuery.Context, ObjectQuery.Context.MetadataWorkspace, MergeOption.AppendOnly, false, true});
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

            HasResult = true;
            Result = list;

            if (ActionTrial != null)
            {
                ActionTrial(list);
            }
        }

        /// <summary>Gets object query.</summary>
        /// <returns>The object query.</returns>
        public virtual ObjectQuery GetObjectQuery()
        {
            if (OriginalQueryable == null && QueryFactory != null)
            {
                OriginalQueryable = QueryFactory(OwnerParent.GetObjectQuery());
            }

            ObjectQuery = OriginalQueryable.GetObjectQuery();
            return OriginalQueryable.GetObjectQuery();
        }

        /// <summary>Creates ordered queryable.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="originalQuery">The original query.</param>
        /// <returns>The new ordered queryable.</returns>
        public virtual BatchOrderedQueryable<TResult> CreateOrderedQueryable<TResult>(IOrderedQueryable<TResult> originalQuery, bool updateChild = true)
        {
            OwnerBatch.Queries.Remove(this);
            var query = new BatchOrderedQueryable<TResult>(originalQuery);
            query.OwnerBatch = OwnerBatch;
            OwnerBatch.Queries.Add(query);

            if (updateChild)
            {
                Childs.ForEach(x => x.OwnerParent = query);
            }
  
            return query;
        }

        /// <summary>Gets the type of the element.</summary>
        /// <value>The type of the element.</value>
        public Type ElementType
        {
            get { return OriginalQueryable.ElementType; }
        }

        /// <summary>Gets the expression.</summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get { return OriginalQueryable.Expression; }
        }

        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public IQueryProvider Provider
        {
            get
            {
                // todo: should be moved in constructor? with nullable InternalProvider ?? OriginalQueryable.Provider
                if (InternalProvider == null)
                {
                    InternalProvider = new BatchQueryProvider(OriginalQueryable.Provider);
                    InternalProvider.CurrentQueryable = this;
                }
                return InternalProvider;
            }
        }


        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (!HasResult)
            {
                OwnerBatch.Execute();
            }

            return (Result as IEnumerable<T>).GetEnumerator();
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return OriginalQueryable.GetEnumerator();
        }
    }
}