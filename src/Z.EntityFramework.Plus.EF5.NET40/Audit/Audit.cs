// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit.</summary>
    public class Audit
    {
        /// <summary>Default constructor.</summary>
        public Audit()
        {
            Configuration = new AuditConfiguration();
            Entries = new List<AuditEntry>();
        }

        /// <summary>Gets or sets the entries.</summary>
        /// <value>The entries.</value>
        public List<AuditEntry> Entries { get; set; }

        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public AuditConfiguration Configuration { get; }
    }
}