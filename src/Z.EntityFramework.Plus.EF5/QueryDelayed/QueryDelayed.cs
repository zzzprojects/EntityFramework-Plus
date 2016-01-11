// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A query delayed.</summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class QueryDelayed<TResult>
    {
        /// <summary>Constructor.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="expression">The expression.</param>
        public QueryDelayed(ObjectQuery source, Expression expression)
        {
            var provider = ((IQueryable) source).Provider;

            var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (Expression), typeof (Type)}, null);
            Source = (IQueryable) createQueryMethod.Invoke(provider, new object[] {expression, typeof (TResult)});
            Expression = expression;
        }

        /// <summary>Gets or sets the expression.</summary>
        /// <value>The expression.</value>
        public Expression Expression { get; protected internal set; }

        /// <summary>Gets or sets the source for the.</summary>
        /// <value>The source.</value>
        public IQueryable Source { get; protected internal set; }

        /// <summary>Gets the execute.</summary>
        /// <returns>A TResult.</returns>
        public TResult Execute()
        {
            return Source.Provider.Execute<TResult>(Expression);
        }

#if NET45
        /// <summary>Executes the asynchronous operation.</summary>
        /// <returns>A Task&lt;TResult&gt;</returns>
        public Task<TResult> ExecuteAsync()
        {
            return Task.Run(() => Execute());
        }
#endif
    }
}