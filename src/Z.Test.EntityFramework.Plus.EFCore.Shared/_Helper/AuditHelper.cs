// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public static class AuditHelper
    {
        public static Audit AutoSaveAudit()
        {
            var audit = new Audit();
            audit.CreatedBy = "ZZZ Projects";
            audit.Configuration.AutoSavePreAction = (context, audit1) => (context as TestContext).AuditEntries.AddRange(audit1.Entries);
            return audit;
        }
    }
}