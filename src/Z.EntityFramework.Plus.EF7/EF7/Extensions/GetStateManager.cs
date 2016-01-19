// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Reflection;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.ChangeTracking.Internal;

namespace Z.EntityFramework.Plus
{
    public static partial class Extensions
    {
        public static StateManager GetStateManager(this ChangeTracker changeTracker)
        {
            var _stateManagerField = changeTracker.GetType().GetField("_stateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            return (StateManager) _stateManagerField.GetValue(changeTracker);
        }
    }
}