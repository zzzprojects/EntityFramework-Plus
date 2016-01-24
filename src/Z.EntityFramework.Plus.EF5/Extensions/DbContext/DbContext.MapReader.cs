// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using Z.EntityFramework.Plus;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

public static partial class DbContextExtensions
{
    public static IEnumerable<T> MapReader<T>(this DbContext context, DbDataReader reader) where T : class
    {
        var list = new List<T>();

        // CREATE a new query to steal the shaper factory from it
        var query = ((IObjectContextAdapter) context).ObjectContext.CreateObjectSet<T>();

        // REFLECTION: Query.QueryState
        var queryStateProperty = query.GetType().GetProperty("QueryState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var queryState = queryStateProperty.GetValue(query, null);

        // REFLECTION: Query.QueryState.GetExecutionPlan(null)
        var getExecutionPlanMethod = queryState.GetType().GetMethod("GetExecutionPlan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var getExecutionPlan = getExecutionPlanMethod.Invoke(queryState, new object[] {null});

        // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory
        var resultShaperFactoryField = getExecutionPlan.GetType().GetField("ResultShaperFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (resultShaperFactoryField == null)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }
        var resultShaperFactory = resultShaperFactoryField.GetValue(getExecutionPlan);

        // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters)
        var createMethod = resultShaperFactory.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if EF5
        var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, query.Context, query.Context.MetadataWorkspace, MergeOption.AppendOnly, false});
#elif EF6
        var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, query.Context, query.Context.MetadataWorkspace, MergeOption.AppendOnly, false, true});
#endif

        // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters).GetEnumerator()
        var getEnumeratorMethod = create.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var getEnumerator = getEnumeratorMethod.Invoke(create, new object[0]);

        var enumerator = (IEnumerator<T>) getEnumerator;

        while (enumerator.MoveNext())
        {
            list.Add(enumerator.Current);
        }
        return list;
    }
}