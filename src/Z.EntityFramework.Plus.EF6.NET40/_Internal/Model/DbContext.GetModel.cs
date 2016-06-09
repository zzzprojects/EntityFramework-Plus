// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.Caching;
using Z.EntityFramework.Plus.Internal;
using Z.EntityFramework.Plus.Internal.Core.Infrastructure;
#if EF5
using System.Data.Metadata.Edm;

#elif EF6
using System.Data.Entity.Core.Metadata.Edm;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
#if BATCH_DELETE || BATCH_UPDATE
        public static string STANDALONE_ID = Guid.NewGuid().ToString();
#endif
        internal static DbModelPlus GetModel(this DbContext context)
        {
            var cache = MemoryCache.Default;

#if BATCH_DELETE || BATCH_UPDATE
            var cacheName = "Z.EntityFramework.Plus.Model;" + STANDALONE_ID + ";" + context.GetType().FullName;
#else
            var cacheName = "Z.EntityFramework.Plus.Model;" + context.GetType().FullName;
#endif


            var model = (DbModelPlus) cache.Get(cacheName);

            if (model == null)
            {
                // GET object context
                var objectContext = ((IObjectContextAdapter) context).ObjectContext;
                var metadataWorkspace = objectContext.MetadataWorkspace;
                EntityContainer entityContainer;

                // CHECK if DbContext is code first
                var isCodeFirst = metadataWorkspace.TryGetEntityContainer("CodeFirstDatabase", true, DataSpace.SSpace, out entityContainer);

                // GET model 
                model = isCodeFirst ? Model.GetCodeFirstModel(context) : Model.GetDatabaseFirst(context);

                cache.Add(new CacheItem(cacheName, model), new CacheItemPolicy());
            }

            return model;
        }
    }
}

#endif
#endif