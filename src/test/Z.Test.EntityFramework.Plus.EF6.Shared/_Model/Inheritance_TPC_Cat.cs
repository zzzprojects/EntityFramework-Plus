// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

namespace Z.Test.EntityFramework.Plus
{
    public class Inheritance_TPC_Cat : Inheritance_TPC_Animal
    {
        public int ColumnCat { get; set; }

        public static Inheritance_TPC_Cat Create()
        {
            return new Inheritance_TPC_Cat();
        }
    }
}