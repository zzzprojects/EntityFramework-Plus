// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;

#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit entry.</summary>
    public class AuditEntry
    {
#if EF7
        public AuditEntry()
        {
            Properties = new List<AuditEntryProperty>();
        }
#endif

        /// <summary>Default constructor.</summary>
        /// <summary>Gets or sets the identifier of the audit entry.</summary>
        /// <value>The identifier of the audit entry.</value>
        public int AuditEntryID { get; set; }

        /// <summary>Gets or sets the entry state.</summary>
        /// <value>The entry state.</value>
        public AuditEntryState State { get; set; }

        /// <summary>Gets or sets the name of the entry state.</summary>
        /// <value>The name of the entry state.</value>
        public string StateName
        {
            get { return State.ToString(); }
            set { State = (AuditEntryState) Enum.Parse(typeof (AuditEntryState), value); }
        }

        /// <summary>Gets or sets the name of the entity set.</summary>
        /// <value>The name of the entity set.</value>
        public string EntitySetName { get; set; }

        /// <summary>Gets or sets the name of the entity type.</summary>
        /// <value>The name of the entity type.</value>
        public string TypeName { get; set; }

        /// <summary>Gets or sets the the date of the changes.</summary>
        /// <value>The date of the changes.</value>
        public DateTime Date { get; set; }

        /// <summary>Gets or sets the user code which made changes.</summary>
        /// <value>The user code which made changes.</value>
        public string UserCode { get; set; }

        /// <summary>Gets or sets the properties.</summary>
        /// <value>The properties.</value>
        public List<AuditEntryProperty> Properties { get; set; }

        // internal object DelayedResolution { get; set; }
        /// <value>The delayed key.</value>
        /// <summary>Gets or sets the delayed key.</summary>
        /// <value>The object state entry.</value>

        /// <summary>Gets or sets the object state entry.</summary>
    }
}