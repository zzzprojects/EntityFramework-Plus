// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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