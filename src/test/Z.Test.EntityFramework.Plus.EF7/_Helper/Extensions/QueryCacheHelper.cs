// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Microsoft.Data.Entity;
using Z.EntityFramework.Plus;
#if EF7
using Microsoft.Extensions.Caching.Memory;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public static class TestExtensions
    {
#if EF7
        public static DbContext GetObjectContext(this DbContext context)
        {
            return context;
        }
#endif
    }
}