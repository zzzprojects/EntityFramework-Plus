// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.ComponentModel.DataAnnotations.Schema;

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit entry property.</summary>
    public class AuditEntryProperty
    {
        /// <summary>Default constructor.</summary>
        public AuditEntryProperty()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="propertyName">Name of the property audited.</param>
        /// <param name="oldValue">The old value audited.</param>
        /// <param name="newValue">The new value audited.</param>
        public AuditEntryProperty(string propertyName, object oldValue, object newValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
            PropertyName = propertyName;
        }

        /// <summary>Gets or sets the identifier of the audit entry property.</summary>
        /// <value>The identifier of the audit entry property.</value>
        public int AuditEntryPropertyID { get; set; }

        /// <summary>Gets or sets the identifier of the audit entry.</summary>
        /// <value>The identifier of the audit entry.</value>
        public int AuditEntryID { get; set; }

        /// <summary>Gets or sets the name of the property audited.</summary>
        /// <value>The name of the property audited.</value>
        public string PropertyName { get; set; }

        /// <summary>Gets or sets the old value string representation audited.</summary>
        /// <value>The old value string representation audited.</value>
        [Column("OldValue")]
        public string OldValueString
        {
            get { return OldValue != null ? OldValue.ToString() : null; }
            set { OldValue = value; }
        }

        /// <summary>Gets or sets the new value string representation audited.</summary>
        /// <value>The new value string representation audited.</value>
        [Column("NewValue")]
        public string NewValueString
        {
            get { return NewValue != null ? NewValue.ToString() : null; }
            set { NewValue = value; }
        }

        /// <summary>Gets or sets the old value audited.</summary>
        /// <value>The old value audited.</value>
        public object OldValue { get; set; }

        /// <summary>Gets or sets the new value audited.</summary>
        /// <value>The new value audited.</value>
        public object NewValue { get; set; }
    }
}