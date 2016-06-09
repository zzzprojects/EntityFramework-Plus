// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using Z.EntityFramework.Plus.Internal.Core.Mapping;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        internal static void BuildMapping(SchemaMapping mapping, Schema conceptual, Schema storage)
        {
            BuildMapping_EntitySet(mapping, conceptual, storage);
        }

        internal static void BuildMapping_EntitySet(SchemaMapping mapping, Schema conceptual, Schema storage)
        {
            mapping.EntityContainerMapping.EntitySetMappings.ForEach(x =>
            {
                // SET Entity Set
                var conceptualSet = conceptual.EntityContainer.Index_EntitySets_Name[x.Name];
                x.ConceptualSet = conceptualSet;

                x.EntityTypeMappings.ForEach(y =>
                {
                    if (y.MappingFragment != null)
                    {
                        var isGenericType = y.TypeName.StartsWith("IsTypeOf(");

                        var typeName = isGenericType ? y.TypeName.SubstringLastIndexOf(".").Replace(")", "") : y.TypeName.SubstringLastIndexOf(".");
                        var entityType = conceptual.Index_EntityTypes_Name[typeName];
                        y.EntityType = entityType;
                        entityType.EntityTypeMapping = y;

                        if (isGenericType)
                        {
                            entityType.GenericEntitySetMapping = y;
                        }

                        var storeEntitySet = storage.EntityContainer.Index_EntitySets_Name[y.MappingFragment.StoreEntitySetName];
                        y.MappingFragment.StoreEntitySet = storeEntitySet;
                    }
                });
            });
        }
    }
}

#endif
#endif