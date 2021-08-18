#if EF6
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    public class QueryCacheItemTracker : IDisposable
    {
        public bool HookAdded { get; set; }
        public ObjectContext Context { get; set; }
        public List<object> MaterializedEntities { get; set; } = new List<object>();

        public void Dispose()
        {
            if (HookAdded)
            {
                Context.ObjectMaterialized -= OnMaterialized;
            }
        }

        public QueryCacheItemTracker Initialize<T>(DbRawSqlQuery<T> query) where T : class
        {
            if (QueryCacheManager.IsAutoExpireCacheEnabled)
            {
                var internalQueryField = typeof(DbRawSqlQuery<T>).GetField("_internalQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (internalQueryField != null)
                {
                    var internalQuery = internalQueryField.GetValue(query);

                    if (internalQuery != null)
                    {
                        object internalContext = null;

                        var internalContextField = internalQuery.GetType().GetField("_internalContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (internalContextField != null)
                        {
                            internalContext = internalContextField.GetValue(internalQuery);
                        }
                        else
                        {
                            var setField = internalQuery.GetType().GetField("_set", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                            if (setField != null)
                            {
                                var set = setField.GetValue(internalQuery);
                                var internalContextProperty = set.GetType().GetProperty("InternalContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                                if (internalContextProperty != null)
                                {
                                    internalContext = internalContextProperty.GetValue(set, null);
                                }
                            }
                        }

                        if (internalContext != null)
                        {
                            var objectContextInitializedProperty = internalContext.GetType().GetProperty("ObjectContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                            if (objectContextInitializedProperty != null)
                            {
                                var objectContextInitialized = objectContextInitializedProperty.GetValue(internalContext, null);

                                if (objectContextInitialized != null && objectContextInitialized is ObjectContext objectContext)
                                {
                                    Context = objectContext;
                                    AddHook();
                                }
                            }
                        }
                    }
                }
            }

            return this;
        }

        public QueryCacheItemTracker Initialize<T>(IQueryable<T> query)
        {
            if (QueryCacheManager.IsAutoExpireCacheEnabled)
            {
                Context = query.GetObjectQuery().Context;
                AddHook();
            }
            else
            {
                try
                {
                    Context = query.GetObjectQuery().Context;
                }
                catch
                {
                    // for backward compatibility, might not always be possible or exists
                }
            }

            return this;
        }

        public QueryCacheItemTracker Initialize<T>(QueryDeferred<T> query)
        {
            if (QueryCacheManager.IsAutoExpireCacheEnabled)
            {
                Context = query.Query.GetObjectQuery().Context;
                AddHook();
            }

            return this;
        }

        public void AddHook()
        {
            Context.ObjectMaterialized += OnMaterialized;
            HookAdded = true;
        }

        public void OnMaterialized(object sender, ObjectMaterializedEventArgs args)
        {
            if (sender == Context)
            {
                MaterializedEntities.Add(args.Entity);
            }
        }
    }
}
#endif