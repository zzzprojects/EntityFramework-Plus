using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Z.EntityFramework.Plus
{
    public class QueryIncludeOrderedQueryable<T, T2> : IOrderedQueryable<T>
    {
        public QueryIncludeProvider<T, T2> IncludeProvider;
        public List<IQueryIncludeQuery> IncludeQueries;
        public IQueryable<T2> IncludeQuery;
        public IQueryable<T> OriginalQuery;

        private IEnumerable<T> Result1;
        public Expression<Func<T, IEnumerable<T2>>> Selector;

        public QueryIncludeOrderedQueryable(IQueryable<T> originalQuery, Expression<Func<T, IEnumerable<T2>>> selector, IQueryable<T2> includeQuery)
        {
            IncludeQueries = new List<IQueryIncludeQuery>();
            IncludeQueries.Add(new QueryIncludeQuery<T, T2>(selector, includeQuery));
            IncludeProvider = new QueryIncludeProvider<T, T2>(originalQuery.Provider, this);
            IncludeQuery = includeQuery;
            OriginalQuery = originalQuery;
            Selector = selector;
        }


        public List<ObjectQuery> Queries { get; set; }

        public ObjectContext Context { get; set; }

        public Type ElementType
        {
            get { return OriginalQuery.ElementType; }
        }

        public Expression Expression
        {
            get { return OriginalQuery.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return IncludeProvider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // GENERATE SQL
            var objectQuery = OriginalQuery.GetObjectQuery();

            //foreach (var query in IncludeQueries)
            //{
            //    var many = OriginalQuery.SelectMany(Selector);
            //}

            //var query2 = OriginalQuery.SelectMany(Selector);
            //var childs = IncludeQuery.Intersect(query2);

            //Context = objectQuery.Context;
            Queries = new List<ObjectQuery>();
            Queries.Add(objectQuery);
            Context = objectQuery.Context;

            foreach (var query in IncludeQueries)
            {
                Queries.Add(query.GetObjectQuery(OriginalQuery));
            }

            ExecuteQueries();

            return Result1.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) OriginalQuery).GetEnumerator();
        }

        public void ExecuteQueries()
        {
            var connection = (EntityConnection) Context.Connection;
            var command = CreateCommand();

            var ownConnection = false;

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    ownConnection = true;
                }

                using (command)
                {
#if EF5
                    using (var reader = command.ExecuteReader())
#elif EF6
                    var interceptionContext = Context.GetInterceptionContext();
                    using (var reader = DbInterception.Dispatch.Command.Reader(command, new DbCommandInterceptionContext(interceptionContext)))
#endif
                    {
                        foreach (var query in Queries)
                        {
                            SetResult(query, reader);
                            reader.NextResult();
                        }
                    }
                }
            }
            finally
            {
                if (ownConnection)
                {
                    connection.Close();
                }
            }
        }

        protected DbCommand CreateCommand()
        {
            var command = Context.CreateStoreCommand();

            var sb = new StringBuilder();
            var queryCount = 1;

            foreach (var query in Queries)
            {
                // GENERATE SQL
                var sql = query.ToTraceString();

                // UPDATE parameter name
                foreach (var parameter in query.Parameters)
                {
                    var oldValue = parameter.Name;
                    var newValue = string.Concat("z", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = newValue;
                    dbParameter.Value = parameter.Value;
                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    sql = sql.Replace("@" + oldValue, "@" + newValue);
                }

                sb.AppendLine(string.Concat("-- Include Query (", queryCount, "/", Queries.Count, ")"));
                sb.AppendLine(sql);
                sb.AppendLine();
                sb.AppendLine();

                queryCount++;
            }

            command.CommandText = sb.ToString();

            return command;
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

            try
            {
                var enumerator = (IEnumerator<T>) getEnumerator;
                var list = new List<T>();

                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }

                Result1 = list;
            }
            catch (Exception)
            {
                var enumerator = (IEnumerator<T2>) getEnumerator;
                var list = new List<T2>();

                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
            }
        }
    }
}