// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Data.Entity.ModelConfiguration;

namespace Z.Test.EntityFramework.Plus
{
    public class Internal_Entity_Basic_Many
    {
        public int ID { get; set; }

        internal int Column1 { get; set; }

        internal int Column2 { get; set; }

        internal int Column3 { get; set; }

        internal class EntityPropertyVisibilityConfiguration : EntityTypeConfiguration<Internal_Entity_Basic_Many>
        {
            public EntityPropertyVisibilityConfiguration()
            {
                Property(pp => pp.Column1);
                Property(pp => pp.Column2);
                Property(pp => pp.Column3);
            }
        }
    }
}
#endif