// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Threading;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit configuration.</summary>
    public class AuditConfiguration
    {
        /// <summary>Default constructor.</summary>
        public AuditConfiguration()
        {
            IncludeEntityAdded = true;
            IncludeEntityDeleted = true;
            IncludeEntityModified = true;
            IncludeRelationAdded = true;
            IncludeRelationDeleted = true;
        }

        /// <summary>Gets or sets the automatic audit save action.</summary>
        /// <value>The automatic audit save action.</value>
        public Action<DbContext, Audit> AutoSaveAction { get; set; }

        /// <summary>Gets or sets the automatic audit save asynchronous action.</summary>
        /// <value>The automatic audit save asynchronous action.</value>
        public Action<DbContext, Audit, CancellationToken> AutoSaveAsyncAction { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Added state are audited.</summary>
        /// <value>true if entity with Added state are audited, false if not.</value>
        public bool IncludeEntityAdded { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Deleted state are audited.</summary>
        /// <value>true if entity with Deleted state are audited, false if not.</value>
        public bool IncludeEntityDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Modified state are audited.</summary>
        /// <value>true if entity with Modified state are audited, false if not.</value>
        public bool IncludeEntityModified { get; set; }

        /// <summary>Gets or sets a value indicating whether the association with Added state are audited.</summary>
        /// <value>true if association with Added state are audited, false if not.</value>
        public bool IncludeRelationAdded { get; set; }

        /// <summary>Gets or sets a value indicating whether the association with Deleted state are audited.</summary>
        /// <value>true if association with Deleted state are audited, false if not.</value>
        public bool IncludeRelationDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether all unchanged property of a modified entity are audited.</summary>
        /// <value>true if unchanged property of a modified entity are audited, false if not.</value>
        public bool IncludePropertyUnchanged { get; set; }

        /// <summary>Gets or sets a function indicating whether the modified entity is soft deleted.</summary>
        /// <value>A function indicating whether the modified entity is soft deleted.</value>
        public Func<object, bool> IsSoftDeleted { get; set; }

        /// <summary>Gets or sets a function indicating whether the modified entity is soft added.</summary>
        /// <value>A function indicating whether the modified entity is soft added.</value>
        public Func<object, bool> IsSoftAdded { get; set; }
    }
}