// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EF7
using Microsoft.Data.Entity.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit entry.</summary>
    public class AuditEntry
    {
        /// <summary>Default constructor.</summary>
        /// <remarks>Required by Entity Framework.</remarks>
        public AuditEntry()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="parent">The audit parent.</param>
        /// <param name="entry">The object state entry.</param>
#if EF5 || EF6
        public AuditEntry(Audit parent, ObjectStateEntry entry)
#elif EF7
        public AuditEntry(Audit parent, EntityEntry entry)
#endif
        {
            CreatedBy = parent.CreatedBy;
            CreatedDate = DateTime.Now;
            Entry = entry;
            Parent = parent;
            Properties = new List<AuditEntryProperty>();

#if EF5 || EF6
            EntitySetName = entry.EntitySet.Name;
            if (!entry.IsRelationship)
            {
                EntityTypeName = entry.Entity.GetType().Name;
            }
#elif EF7
            EntityTypeName = Entry.Entity.GetType().Name;
#endif
        }

        /// <summary>Gets or sets the identifier of the audit entry.</summary>
        /// <value>The identifier of the audit entry.</value>
        [Column(Order = 0)]
        public int AuditEntryID { get; set; }

        /// <summary>Gets or sets who created this object.</summary>
        /// <value>Describes who created this object.</value>
        [Column(Order = 5)]
        [MaxLength(255)]
        public string CreatedBy { get; set; }

        /// <summary>Gets or sets the the date of the changes.</summary>
        /// <value>The date of the changes.</value>
        [Column(Order = 6)]
        public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the delayed key.</summary>
        /// <value>The delayed key.</value>
#if EF5 || EF6
        [NotMapped]
        internal object DelayedKey { get; set; }
#elif EF7
        internal object DelayedKey;
#endif

        /// <summary>Gets or sets the object state entry.</summary>
        /// <value>The object state entry.</value>
        [NotMapped]
#if EF5 || EF6
            public ObjectStateEntry Entry { get; set; }
#elif EF7
    // EF7 still have some issue with "NotMapped" attribute
        public EntityEntry Entry { get; set; }
#endif

        /// <summary>Gets or sets the name of the entity set.</summary>
        /// <value>The name of the entity set.</value>
        [Column(Order = 1)]
        [MaxLength(255)]
        public string EntitySetName { get; set; }

        /// <summary>Gets or sets the name of the entity type.</summary>
        /// <value>The name of the entity type.</value>
        [Column(Order = 2)]
        [MaxLength(255)]
        public string EntityTypeName { get; set; }

        /// <summary>Gets or sets the parent.</summary>
        /// <value>The parent.</value>
#if EF5 || EF6
        [NotMapped]
        public Audit Parent { get; set; }
#elif EF7
    // EF7 still have some issue with "NotMapped" attribute
        public Audit Parent;
#endif

        /// <summary>Gets or sets the properties.</summary>
        /// <value>The properties.</value>
        public List<AuditEntryProperty> Properties { get; set; }

        /// <summary>Gets or sets the entry state.</summary>
        /// <value>The entry state.</value>
        [Column(Order = 3)]
        public AuditEntryState State { get; set; }

        /// <summary>Gets or sets the name of the entry state.</summary>
        /// <value>The name of the entry state.</value>
        [Column(Order = 4)]
        [MaxLength(255)]
        public string StateName
        {
            get { return State.ToString(); }
            set { State = (AuditEntryState) Enum.Parse(typeof (AuditEntryState), value); }
        }
    }
}