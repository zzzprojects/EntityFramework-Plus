// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if STANDALONE
namespace Z.EntityFramework.Plus
{
    internal class ExceptionMessage
    {
        public static string GeneralException = "Oops! A general error has occurred. Please report the issue including the stack trace to our support team: info@zzzprojects.com";
        public static string BatchOperations_MaxKeyColumns = "Oops! Batch operation only support table with primary keys containing 5 columns or less. For more information, contact us: info@zzzprojects.com";
        public static string BatchOperations_PropertyNotFound = "Oops! Mapping for the property '{0}' cannot be found, the current version don't support yet Complex Type, Enum, TPC, TPH and TPT. For more information, contact us: info@zzzprojects.com";
    }
}
#endif