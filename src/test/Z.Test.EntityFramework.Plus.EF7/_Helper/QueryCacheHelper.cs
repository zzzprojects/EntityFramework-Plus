// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using Z.EntityFramework.Plus;
#if EF7
using Microsoft.Extensions.Caching.Memory;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public static class QueryCacheHelper
    {
        public static long GetCacheCount()
        {
#if EF5 || EF6
            return QueryCacheManager.Cache.GetCount();
#elif EF7
            return ((MemoryCache) QueryCacheManager.Cache).Count;
#endif
        }
    }
}