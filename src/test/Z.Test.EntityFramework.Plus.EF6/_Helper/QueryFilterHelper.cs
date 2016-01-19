// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public static class QueryFilterHelper
    {
        public enum Filter
        {
            Filter1,
            Filter2,
            Filter3,
            Filter4,
            Filter5,
            Filter6,
            Filter7,
            Filter8
        }

        public static void ClearGlobalManagerFilter()
        {
            QueryFilterManager.GlobalFilters.Clear();
            QueryFilterManager.GlobalInitializeFilterActions.Clear();
        }

        public static void CreateGlobalManagerFilter(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null)
        {
#if EF7
    // TODO: Remove this when cast issue will be fixed
            QueryFilterManager.GlobalFilters.Clear();
            QueryFilterManager.GlobalInitializeFilterActions.Clear();
#endif

            if (enableFilter1 != null)
            {
                QueryFilterManager.Filter<Inheritance_Interface_Entity>(Filter.Filter1, entities => entities.Where(x => x.ColumnInt != 1), isEnabled);
                if (!isEnabled && enableFilter1.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter1).IsDefaultEnabled = true;
                }
                else if (isEnabled && !enableFilter1.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter1).IsDefaultEnabled = false;
                }
            }
            if (enableFilter2 != null)
            {
                QueryFilterManager.Filter<Inheritance_Interface_IEntity>(Filter.Filter2, entities => entities.Where(x => x.ColumnInt != 2), isEnabled);
                if (!isEnabled && enableFilter2.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter2).IsDefaultEnabled = true;
                }
                else if (isEnabled && !enableFilter2.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter2).IsDefaultEnabled = false;
                }
            }
            if (enableFilter3 != null)
            {
                QueryFilterManager.Filter<Inheritance_Interface_Base>(Filter.Filter3, entities => entities.Where(x => x.ColumnInt != 3), isEnabled);
                if (!isEnabled && enableFilter3.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter3).IsDefaultEnabled = true;
                }
                else if (isEnabled && !enableFilter3.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter3).IsDefaultEnabled = false;
                }
            }
            if (enableFilter4 != null)
            {
                QueryFilterManager.Filter<Inheritance_Interface_IBase>(Filter.Filter4, entities => entities.Where(x => x.ColumnInt != 4), isEnabled);
                if (!isEnabled && enableFilter4.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter4).IsDefaultEnabled = true;
                }
                else if (isEnabled && !enableFilter4.Value)
                {
                    QueryFilterManager.Filter(Filter.Filter4).IsDefaultEnabled = false;
                }
            }

            if (excludeClass != null && excludeClass.Value)
            {
                QueryFilterManager.Filter(Filter.Filter1).Disable(typeof (Inheritance_Interface_Entity));
            }

            if (excludeInterface != null && excludeInterface.Value)
            {
                QueryFilterManager.Filter(Filter.Filter2).Disable(typeof (Inheritance_Interface_IEntity));
            }

            if (excludeBaseClass != null && excludeBaseClass.Value)
            {
                QueryFilterManager.Filter(Filter.Filter3).Disable(typeof (Inheritance_Interface_Base));
            }

            if (excludeBaseInterface != null && excludeBaseInterface.Value)
            {
                QueryFilterManager.Filter(Filter.Filter4).Disable(typeof (Inheritance_Interface_IBase));
            }

            if (includeClass != null && includeClass.Value)
            {
                QueryFilterManager.Filter(Filter.Filter1).Enable(typeof (Inheritance_Interface_IEntity));
            }

            if (includeInterface != null && includeInterface.Value)
            {
                QueryFilterManager.Filter(Filter.Filter2).Enable(typeof (Inheritance_Interface_IEntity));
            }

            if (includeBaseClass != null && includeBaseClass.Value)
            {
                QueryFilterManager.Filter(Filter.Filter3).Enable(typeof (Inheritance_Interface_Base));
            }

            if (includeBaseInterface != null && includeBaseInterface.Value)
            {
                QueryFilterManager.Filter(Filter.Filter4).Enable(typeof (Inheritance_Interface_IBase));
            }
        }
    }
}