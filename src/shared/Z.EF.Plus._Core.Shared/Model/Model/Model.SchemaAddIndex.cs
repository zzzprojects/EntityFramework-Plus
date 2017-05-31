// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Linq;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        internal static void SchemaAddIndex(Schema schema)
        {
            schema.Index_EntityTypes_Name = schema.EntityTypes.ToDictionary(x => x.Name);

            schema.EntityContainer.Index_EntitySets_Name = schema.EntityContainer.EntitySets.ToDictionary(x => x.Name);

            schema.EntityTypes.ForEach(x => { x.Index_Properties_Name = x.Properties.ToDictionary(y => y.Name); });
        }
    }
}

#endif
#endif