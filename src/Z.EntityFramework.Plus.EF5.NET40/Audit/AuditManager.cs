// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for audits.</summary>
    public class AuditManager
    {
        /// <summary>Gets or sets the default audit configuration.</summary>
        /// <value>The default audit configuration.</value>
        public AuditConfiguration DefaultConfiguration { get; set; }
    }
}