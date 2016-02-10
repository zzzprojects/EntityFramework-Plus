// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

namespace Z.EntityFramework.Plus
{
    /// <summary>Values that represent audit entry states.</summary>
    public enum AuditEntryState
    {
        /// <summary>An enum constant representing the entity added option.</summary>
        EntityAdded,

        /// <summary>An enum constant representing the entity deleted option.</summary>
        EntityDeleted,

        /// <summary>An enum constant representing the entity modified option.</summary>
        EntityModified,

        /// <summary>An enum constant representing the entity soft added option.</summary>
        EntitySoftAdded,

        /// <summary>An enum constant representing the entity soft deleted option.</summary>
        EntitySoftDeleted,

        /// <summary>An enum constant representing the relationship added option.</summary>
        RelationshipAdded,

        /// <summary>An enum constant representing the relationship deleted option.</summary>
        RelationshipDeleted
    }
}