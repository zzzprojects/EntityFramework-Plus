// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit entry property.</summary>
    public class AuditEntryProperty
    {
        /// <summary>Default constructor.</summary>
        /// <remarks>Required by Entity Framework.</remarks>
        public AuditEntryProperty()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="parent">The audit entry parent.</param>
        /// <param name="propertyName">Name of the property audited.</param>
        /// <param name="oldValue">The old value audited.</param>
        /// <param name="newValue">The new value audited.</param>
        public AuditEntryProperty(AuditEntry parent, string propertyName, object oldValue, object newValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
            Parent = parent;
            PropertyName = propertyName;
        }

        /// <summary>Constructor.</summary>
        /// <param name="parent">The audit entry parent.</param>
        /// <param name="relationName">The name of the relation audited.</param>
        /// <param name="propertyName">Name of the property audited.</param>
        /// <param name="oldValue">The old value audited.</param>
        /// <param name="newValue">The new value audited.</param>
        public AuditEntryProperty(AuditEntry parent, string relationName, string propertyName, object oldValue, object newValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
            Parent = parent;
            PropertyName = propertyName;
            RelationName = relationName;
        }

        /// <summary>Gets or sets the identifier of the audit entry property.</summary>
        /// <value>The identifier of the audit entry property.</value>
        [Column(Order = 0)]
        public int AuditEntryPropertyID { get; set; }

        /// <summary>Gets or sets the identifier of the audit entry.</summary>
        /// <value>The identifier of the audit entry.</value>
        [Column(Order = 1)]
        public int AuditEntryID { get; set; }

        /// <summary>Gets or sets the parent.</summary>
        /// <value>The parent.</value>
        public AuditEntry Parent { get; set; }

        /// <summary>Gets or sets the name of the property audited.</summary>
        /// <value>The name of the property audited.</value>
        [Column(Order = 3)]
        [MaxLength(255)]
        public string PropertyName { get; set; }

        /// <summary>Gets or sets the name of the relation audited.</summary>
        /// <value>The name of the relation audited.</value>
        [Column(Order = 2)]
        [MaxLength(255)]
        public string RelationName { get; set; }

        /// <summary>Gets or sets the new value audited.</summary>
        /// <value>The new value audited.</value>
#if EF5 || EF6
        [NotMapped]
        public object NewValue { get; set; }
#elif EF7
        // EF7 still have some issue with "NotMapped" attribute
        public object NewValue;
#endif

        /// <summary>Gets or sets the new value audited formatted.</summary>
        /// <value>The new value audited formatted.</value>
        [Column("NewValue", Order = 5)]
        public string NewValueFormatted
        {
            get
            {
                var currentValue = NewValue;

                if (Parent != null && Parent.Parent != null && Parent.State != AuditEntryState.EntityDeleted)
                {
                    return Parent.Parent.CurrentOrDefaultConfiguration.FormatValue(Parent.Entry, PropertyName, currentValue);
                }

                return currentValue != null ? currentValue.ToString() : null;
            }
            set { NewValue = value; }
        }

#if EF5 || EF6
    /// <summary>Gets or sets the old value audited.</summary>
    /// <value>The old value audited.</value>
        [NotMapped]
        public object OldValue { get; set; }
#elif EF7
        // EF7 still have some issue with "NotMapped" attribute
        public object OldValue;
#endif

        /// <summary>Gets or sets the old value audited formatted.</summary>
        /// <value>The old value audited formatted.</value>
        [Column("OldValue", Order = 4)]
        public string OldValueFormatted
        {
            get
            {
                var currentValue = OldValue;

                if (Parent != null && Parent.Parent != null && Parent.State != AuditEntryState.EntityAdded)
                {
                    return Parent.Parent.CurrentOrDefaultConfiguration.FormatValue(Parent.Entry, PropertyName, currentValue);
                }

                return currentValue != null ? currentValue.ToString() : null;
            }
            set { OldValue = value; }
        }
    }
}