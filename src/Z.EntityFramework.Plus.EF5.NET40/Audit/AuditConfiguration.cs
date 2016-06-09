// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit configuration.</summary>
    public partial class AuditConfiguration
    {
        /// <summary>Default constructor.</summary>
        public AuditConfiguration()
        {
            IgnorePropertyUnchanged = true;

            EntityValueFormatters = new List<Func<object, string, Func<object, object>>>();
            ExcludeIncludeEntityPredicates = new List<Func<object, bool?>>();
            ExcludeIncludePropertyPredicates = new List<Func<object, string, bool?>>();

            SoftAddedPredicates = new List<Func<object, bool>>();
            SoftDeletedPredicates = new List<Func<object, bool>>();

            IsAuditedDictionary = new ConcurrentDictionary<string, bool>();
            ValueFormatterDictionary = new ConcurrentDictionary<string, Func<object, object>>();
        }

        /// <summary>Gets or sets the automatic audit save pre action.</summary>
        /// <value>The automatic audit save pre action.</value>
        public Action<DbContext, Audit> AutoSavePreAction { get; set; }

        /// <summary>Gets or sets a list of formatter for entity values.</summary>
        /// <value>A list of formatter for entity values.</value>
        public List<Func<object, string, Func<object, object>>> EntityValueFormatters { get; set; }

        /// <summary>Gets or sets a list of predicates to exclude or include entities.</summary>
        /// <value>A list of predicates to exclude or include entities.</value>
        public List<Func<object, bool?>> ExcludeIncludeEntityPredicates { get; set; }

        /// <summary>Gets or sets a list of predicates to exclude or include properties.</summary>
        /// <value>A list of predicates to exclude or include properties.</value>
        public List<Func<object, string, bool?>> ExcludeIncludePropertyPredicates { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Added state are audited.</summary>
        /// <value>true if entity with Added state are audited, false if not.</value>
        public bool IgnoreEntityAdded { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Deleted state are audited.</summary>
        /// <value>true if entity with Deleted state are audited, false if not.</value>
        public bool IgnoreEntityDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether the entity with Modified state are audited.</summary>
        /// <value>true if entity with Modified state are audited, false if not.</value>
        public bool IgnoreEntityModified { get; set; }

        /// <summary>Gets or sets a value indicating whether the ignore entity soft added.</summary>
        /// <value>true if ignore entity soft added, false if not.</value>
        public bool IgnoreEntitySoftAdded { get; set; }

        /// <summary>Gets or sets a value indicating whether the ignore entity soft deleted.</summary>
        /// <value>true if ignore entity soft deleted, false if not.</value>
        public bool IgnoreEntitySoftDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether the association with Added state are audited.</summary>
        /// <value>true if association with Added state are audited, false if not.</value>
        public bool IgnoreRelationshipAdded { get; set; }

        /// <summary>Gets or sets a value indicating whether the association with Deleted state are audited.</summary>
        /// <value>true if association with Deleted state are audited, false if not.</value>
        public bool IgnoreRelationshipDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether all unchanged property of a modified entity are audited.</summary>
        /// <value>true if unchanged property of a modified entity are audited, false if not.</value>
        public bool IgnorePropertyUnchanged { get; set; }

        /// <summary>Gets or sets a dictionary indicating if an entity type or a property name is audited.</summary>
        /// <value>A dictionary indicating if an entity type or a property name is audited.</value>
        public ConcurrentDictionary<string, bool> IsAuditedDictionary { get; set; }

        /// <summary>Gets or sets a list of predicates to check if the modified entity is soft added.</summary>
        /// <value>A list of predicates to check if the modified entity is soft added.</value>
        public List<Func<object, bool>> SoftAddedPredicates { get; set; }

        /// <summary>Gets or sets a list of predicates to check if the modified entity is soft deleted.</summary>
        /// <value>A list of predicates to check if the modified entity is soft deleted.</value>
        public List<Func<object, bool>> SoftDeletedPredicates { get; set; }

        /// <summary>Gets or sets a dictionary of value formatter for a property name.</summary>
        /// <value>A dictionary of value formatter for a property name.</value>
        public ConcurrentDictionary<string, Func<object, object>> ValueFormatterDictionary { get; set; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        public AuditConfiguration Clone()
        {
            var audit = new AuditConfiguration
            {
                AutoSavePreAction = AutoSavePreAction,
                IgnoreEntityAdded = IgnoreEntityAdded,
                IgnoreEntityModified = IgnoreEntityModified,
                IgnoreEntityDeleted = IgnoreEntityDeleted,
                IgnoreEntitySoftAdded = IgnoreEntitySoftAdded,
                IgnoreEntitySoftDeleted = IgnoreEntitySoftDeleted,
                IgnorePropertyUnchanged = IgnorePropertyUnchanged,
                IgnoreRelationshipAdded = IgnoreRelationshipAdded,
                IgnoreRelationshipDeleted = IgnoreRelationshipDeleted,
                EntityValueFormatters = new List<Func<object, string, Func<object, object>>>(EntityValueFormatters),
                ExcludeIncludeEntityPredicates = new List<Func<object, bool?>>(ExcludeIncludeEntityPredicates),
                ExcludeIncludePropertyPredicates = new List<Func<object, string, bool?>>(ExcludeIncludePropertyPredicates),
                SoftAddedPredicates = new List<Func<object, bool>>(SoftAddedPredicates),
                SoftDeletedPredicates = new List<Func<object, bool>>(SoftDeletedPredicates)
            };

            return audit;
        }
    }
}