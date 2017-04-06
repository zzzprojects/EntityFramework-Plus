using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class to manage query interceptor filter options.</summary>
    public static class QueryFilterManager
    {
        /// <summary>A filter specifying the prefix.</summary>
        internal static string PrefixFilter = "Z.EntityFramework.Plus.QueryFilterInterceptor.Filer;";

        /// <summary>The prefix hook.</summary>
        internal static string PrefixHook = "Z.EntityFramework.Plus.QueryFilterInterceptor.Hook;";

        /// <summary>A filter specifying the disable all.</summary>
        internal static string DisableAllFilter = PrefixFilter + "DisableAll;";

        /// <summary>Identifier for the enable filter by.</summary>
        internal static string EnableFilterById = PrefixFilter + "EnableById;";
        internal static string PrefixFilterID = "Z.EntityFramework.Plus.QueryFilterInterceptor.FilterID;";

        internal static List<Type> Types = new List<Type>(); 

        /// <summary>Static constructor.</summary>
        static QueryFilterManager()
        {
            DbInterception.Add(new QueryFilterInterceptorDbCommandTree());

            CacheWeakFilterContext = new ConditionalWeakTable<DbContext, QueryFilterContextInterceptor>();
            GlobalFiltersByKey = new ConcurrentDictionary<object, BaseQueryFilterInterceptor>();
            GlobalFilterByType = new ConcurrentDictionary<Type, List<BaseQueryFilterInterceptor>>();
            DbExpressionByHook = new ConcurrentDictionary<string, DbExpression>();
            DbExpressionParameterByHook = new ConcurrentDictionary<DbExpression, DbParameterCollection>();
        }

        public static void RegisterType(params Type[] types)
        {
            Types = Types.Union(types).ToList();
        }

        public static void RegisterType(Assembly assembly)
        {
            var types = assembly.GetTypes();
            RegisterType(types);
        }


        /// <summary>Gets the database expression by hook.</summary>
        /// <value>The database expression by hook.</value>
        public static ConcurrentDictionary<string, DbExpression> DbExpressionByHook { get; }

        /// <summary>Gets the database expression parameter by hook.</summary>
        /// <value>The database expression parameter by hook.</value>
        public static ConcurrentDictionary<DbExpression, DbParameterCollection> DbExpressionParameterByHook { get; }

        /// <summary>Gets the global filters.</summary>
        /// <value>The global filters.</value>
        public static ConcurrentDictionary<object, BaseQueryFilterInterceptor> GlobalFiltersByKey { get; }

        /// <summary>Gets or sets the type of the global filter by.</summary>
        /// <value>The type of the global filter by.</value>
        public static ConcurrentDictionary<Type, List<BaseQueryFilterInterceptor>> GlobalFilterByType { get; set; }

        /// <summary>Gets or sets the weak table containing filter context for a specified context.</summary>
        /// <value>The weak table containing filter context for a specified context.</value>
        public static ConditionalWeakTable<DbContext, QueryFilterContextInterceptor> CacheWeakFilterContext { get; set; }

        /// <summary>Gets the filter associated with the specified key from the context.</summary>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <returns>The filter associated with the specified key from the context.</returns>
        public static BaseQueryFilterInterceptor Filter(object key)
        {
            BaseQueryFilterInterceptor filter;
            GlobalFiltersByKey.TryGetValue(key, out filter);

            return filter;
        }

        /// <summary>
        ///     Creates and return a filter added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilterInterceptor Filter<T>(Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true) where T : class
        {
            return Filter(Guid.NewGuid(), queryFilter, isEnabled);
        }

        /// <summary>
        ///     Creates and return a filter associated with the specified key added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilterInterceptor Filter<T>(object key, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true) where T : class
        {
            BaseQueryFilterInterceptor filter;

            // FilterByKey
            {
                if (!GlobalFiltersByKey.TryGetValue(key, out filter))
                {
                    filter = new QueryFilterInterceptor<T>(queryFilter) {IsDefaultEnabled = isEnabled};
                    GlobalFiltersByKey.AddOrUpdate(key, filter, (o, interceptorFilter) => filter);
                }
            }

            // FilterByType
            {
                if (!GlobalFilterByType.ContainsKey(typeof (T)))
                {
                    GlobalFilterByType.AddOrUpdate(typeof (T), new List<BaseQueryFilterInterceptor>(), (type, list) => list);
                }

                GlobalFilterByType[typeof (T)].Add(filter);
            }

            ClearAllCache();

            return filter;
        }

        /// <summary>Adds or get the filter context associated with the context.</summary>
        /// <param name="context">The context associated with the filter context.</param>
        /// <returns>The filter context associated with the context.</returns>
        public static QueryFilterContextInterceptor AddOrGetFilterContext(DbContext context)
        {
            QueryFilterContextInterceptor filterContext;

            if (!CacheWeakFilterContext.TryGetValue(context, out filterContext))
            {
                // EnsureAlwaysEmptyDictionary(context);

                filterContext = new QueryFilterContextInterceptor(context)
                {
                    GlobalFilterByKey = GlobalFiltersByKey,
                    GlobalFilterByType = GlobalFilterByType
                };
                CacheWeakFilterContext.Add(context, filterContext);
            }

            return filterContext;
        }

        /// <summary>Hook filter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> HookFilter<T>(IQueryable<T> query, string value)
        {
            // CREATE hook
            var parameter = Expression.Parameter(typeof (T));
            var left = Expression.Constant(value);
            var right = Expression.Constant(value);
            var predicate = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);

            // APPLY hook
            query = query.Where(lambda);

            return query;
        }

        public static IQueryable HookFilter2(IQueryable query, Type type, string value)
        {
            var method = typeof(QueryFilterManager).GetMethod("HookFilter").MakeGenericMethod(type);
            var result = method.Invoke(null, new object[] {query, value});

            return (IQueryable)result;
        }

        /// <summary>Initilize global filter in the context.</summary>
        /// <param name="context">The context to initialize global filter on.</param>
        public static void InitilizeGlobalFilter(DbContext context)
        {
            // Initialize global filter
            var filterContext = AddOrGetFilterContext(context);
            filterContext.UpdateHook(context);
        }

        /// <summary>Clears the query cache described by context.</summary>
        /// <param name="context">The context to initialize global filter on.</param>
        public static void ClearQueryCache(DbContext context)
        {
            try
            {
                var objectContext = context.GetObjectContext();
                var metaWorkspace = objectContext.MetadataWorkspace;

                var getQueryCacheManagerMethod = metaWorkspace.GetType().GetMethod("GetQueryCacheManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var getQueryCacheManager = getQueryCacheManagerMethod.Invoke(metaWorkspace, null);

                var clearMethod = getQueryCacheManager.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                clearMethod.Invoke(getQueryCacheManager, null);

                // GET DbSet<> properties
                var setProperties = context.GetDbSetProperties();

                foreach (var setProperty in setProperties)
                {
                    var dbSet = (IQueryable)setProperty.GetValue(context, null);

                    // Disable Set Caching
                    {
                        var objectQuery = dbSet.GetObjectQuery();

                        var stateField = objectQuery.GetType().BaseType.GetField("_state", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var state = stateField.GetValue(objectQuery);

                        var cachedPlanField = state.GetType().GetField("_cachedPlan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        cachedPlanField.SetValue(state, null);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Find a better way to handle this!
            }
        }

        /// <summary>Ensures that always empty dictionary.</summary>
        /// <param name="context">The context to initialize global filter on.</param>
        public static void ForceAlwaysEmptyQueryCacheManager(DbContext context)
        {
            var objectContext = context.GetObjectContext();
            var metaWorkspace = objectContext.MetadataWorkspace;

            var getQueryCacheManagerMethod = metaWorkspace.GetType().GetMethod("GetQueryCacheManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getQueryCacheManager = getQueryCacheManagerMethod.Invoke(metaWorkspace, null);

            var cacheDataField = getQueryCacheManager.GetType().GetField("_cacheData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var cacheData = cacheDataField.GetValue(getQueryCacheManager);

            var alwaysEmptyDictionaryType = typeof(QueryFilterAlwaysEmptyDictionary<,>);
            var alwaysEmptyDictionaryGenericType = alwaysEmptyDictionaryType.MakeGenericType(cacheData.GetType().GetGenericArguments());

            if (cacheData.GetType() != alwaysEmptyDictionaryGenericType)
            {
                var comparerType = typeof(QueryFilterAlwaysEmptyDictionaryComparer<,>);
                var comparerGenericType = comparerType.MakeGenericType(cacheData.GetType().GetGenericArguments());
                var comparer = Activator.CreateInstance(comparerGenericType);

                var constructor = alwaysEmptyDictionaryGenericType.GetConstructor(new Type[] { comparerGenericType });

                var alwaysEmptyDictionary = constructor.Invoke(new object[] { comparer });

                cacheDataField.SetValue(getQueryCacheManager, alwaysEmptyDictionary);
            }
        }

        /// <summary>Clears all cache.</summary>
        public static void ClearAllCache()
        {
            var propertyValues = CacheWeakFilterContext.GetType().GetProperty("Values", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var values = (List<QueryFilterContextInterceptor>) propertyValues.GetValue(CacheWeakFilterContext, null);

            foreach (var value in values)
            {
                value.ClearCache();
            }
        }

        /// <summary>Clears the global filter.</summary>
        public static void ClearGlobalFilter()
        {
            GlobalFiltersByKey.Clear();
            GlobalFilterByType.Clear();
        }
    }
}