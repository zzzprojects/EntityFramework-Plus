// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public static class AuditHelper
    {
        public static Audit AutoSaveAudit()
        {
            var audit = new Audit();
#if EF5 || EF6
            audit.Configuration.AutoSaveAction = (context, audit1) => (context as TestContext).AuditEntries.AddRange(audit.Entries);
#endif

            return audit;
        }
    }
}