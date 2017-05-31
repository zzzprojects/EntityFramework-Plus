// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        internal static void BuildSchema(Schema schema, bool isStorage = false)
        {
            // Index Schema
            SchemaAddIndex(schema);

            BuildSchema_EntityType(schema, isStorage);
            BuildSchema_EntitySet(schema, isStorage);
        }

        internal static void BuildSchema_EntityType(Schema schema, bool isStorage = false)
        {
            schema.EntityTypes.ForEach(x =>
            {
                // SET Primary Key
                if (x.Key != null)
                {
                    x.Key.PropertyRefs.ForEach(y => y.Property = x.Index_Properties_Name[y.Name]);
                }
            });
        }

        internal static void BuildSchema_EntitySet(Schema schema, bool isStorage = false)
        {
            // SET Entity Type
            schema.EntityContainer.EntitySets.ForEach(x =>
            {
                var typeName = x.EntityTypeName.SubstringLastIndexOf(".");
                var entityType = schema.Index_EntityTypes_Name[typeName];
                x.EntityType = entityType;
                entityType.EntitySet = x;
            });

            // Database First doesn't map by default the table name because it's always equal to the entity set.
            if (isStorage)
            {
                schema.EntityContainer.EntitySets.ForEach(x => { x.Table = string.IsNullOrEmpty(x.Table) ? x.Name : x.Table; });
            }
        }
    }
}

#endif
#endif