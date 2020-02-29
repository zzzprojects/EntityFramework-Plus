//// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
//// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
//// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
//// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
//// More projects: http://www.zzzprojects.com/
//// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

//#if FULL || BATCH_DELETE || BATCH_UPDATE
//#if EF5 || EF6
//using System;
//using System.Linq;
//using Z.EF.Plus.BatchUpdate.Shared.Extensions;
//using Z.EntityFramework.Extensions.Core.Mapping;
//using Z.EntityFramework.Plus.Internal.Core.Mapping;
//using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

//namespace Z.EntityFramework.Plus.Internal
//{
//    internal static partial class Model
//    {
//        internal static void BuildMapping(Z.EntityFramework.Plus.Internal.Core.Mapping.SchemaMapping mapping, Schema conceptual, Schema storage)
//        {
//            BuildMapping_EntitySet(mapping, conceptual, storage);
//	        BuildMapping_EntitySetMapping(mapping, conceptual, storage);
//	        BuildMapping_FixTPH(mapping, conceptual, storage);
//		}

//        internal static void BuildMapping_EntitySet(Z.EntityFramework.Plus.Internal.Core.Mapping.SchemaMapping mapping, Schema conceptual, Schema storage)
//        {
//			mapping.EntityContainerMapping.EntitySetMappings.ForEach(x =>
//			{
//				// SET Entity Set
//				var conceptualSet = conceptual.EntityContainer.Index_EntitySets_Name[x.Name];
//				x.ConceptualSet = conceptualSet;
//				conceptualSet.EntitySetMapping = x;

//				x.EntityTypeMappings.ForEach(y =>
//				{
//					if (y.MappingFragment != null)
//					{
//						var isGenericType = y.TypeName.StartsWith("IsTypeOf(", StringComparison.InvariantCulture);

//						var typeName = isGenericType ? y.TypeName.SubstringLastIndexOf(".").Replace(")", "") : y.TypeName.SubstringLastIndexOf(".");
//						var entityType = conceptual.Index_EntityTypes_Name[typeName];
//						y.EntityType = entityType;
//						entityType.EntityTypeMapping = y;

//						if (isGenericType)
//						{
//							entityType.GenericEntitySetMapping = y;
//						}

//						var storeEntitySet = storage.EntityContainer.Index_EntitySets_Name[y.MappingFragment.StoreEntitySetName];
//						storeEntitySet.EntitySetMapping = x;
//						y.MappingFragment.StoreEntitySet = storeEntitySet;
//					}
//					else
//					{
//						// FunctionMapping
//					}
//				});

//				var entityTypeMapping = x.EntityTypeMappings.Where(y => y.MappingFragment != null).ToList();
//				if (entityTypeMapping.Count > 1)
//				{
//					if (entityTypeMapping.Select(y => y.MappingFragment.StoreEntitySetName).Distinct().Count() == 1)
//					{
//						// TPH => All entity saved in the same table
//						entityTypeMapping.ForEach(y => y.EntityType.IsTPH = true);
//						x.ConceptualSet.IsTPH = true;
//					}
//					else if (!entityTypeMapping.Exists(y => y.TypeName.StartsWith("IsTypeOf(", StringComparison.InvariantCulture)))
//					{
//						// TPC => All most derived entity saved in their own table
//						entityTypeMapping.ForEach(y => y.EntityType.IsTPC = true);
//						x.ConceptualSet.IsTPC = true;
//					}
//					else
//					{
//						// TPT => All entity saved in their own table
//						entityTypeMapping.ForEach(y => y.EntityType.IsTPT = true);
//						x.ConceptualSet.IsTPT = true;
//					}
//				}
//			});
//		}

//		internal static void BuildMapping_EntitySetMapping(Z.EntityFramework.Plus.Internal.Core.Mapping.SchemaMapping mapping, Schema conceptual, Schema storage)
//		{
//			mapping.EntityContainerMapping.EntitySetMappings.ForEach(entitySetMapping =>
//			{
//				entitySetMapping.EntityTypeMappings.Where(x => x.MappingFragment != null).ToList().ForEach(entityTypeMapping =>
//				{
//					var mappingFragment = entityTypeMapping.MappingFragment;

//					// ADD scalar accessor for properties
//					mappingFragment.ScalarProperties.ForEach(x =>
//					{
//						Type type = null;
//						if (entityTypeMapping.EntityType != null)
//						{
//							type = GetPropertyType_Reccursive(entityTypeMapping.EntityType, x.Name);
//						}

//						var property = GetProperty_Reccursive(entityTypeMapping.EntityType, x.Name);
//						mappingFragment.ScalarAccessors.Add(new ScalarAccessorMapping
//						{
//							IsStorageMapped = true,
//							Type = type,
//							AccessorPath = x.Name,
//							ColumnName = x.ColumnName,
//							IsComputed = property.IsComputed,
//							IsConcurrency = property.IsConcurrency,
//							IsIdentity = property.StoreGeneratedPattern == "Identity"
//						});
//					});

//					// ADD scalar accessor from conditions
//					mappingFragment.Conditions.ForEach(x => mappingFragment.ScalarAccessors.Add(new ScalarAccessorMapping
//					{
//						IsStorageMapped = true,
//						Type = typeof(string),
//						AccessorPath = x.ColumnName,
//						ConstantValue = x.Value,
//						ColumnName = x.ColumnName
//					}));
//				});
//			});
//		}

//		internal static Type GetPropertyType_Reccursive(SchemaEntityType entityType, string propertyName)
//	    {
//		    var property = entityType.Index_Properties_Name.GetValueOrNull(propertyName);
//		    if (property != null)
//		    {
//			    var type = property.IsEnum ? property.EnumType.Type : property.Type;
//			    return type;
//		    }
//		    if (entityType.BaseType != null)
//		    {
//			    return GetPropertyType_Reccursive(entityType.BaseType, propertyName);
//		    }
//		    throw new Exception("Could not found the property.");
//	    }

//	    internal static Property GetProperty_Reccursive(SchemaEntityType entityType, string propertyName)
//	    {
//		    var property = entityType.Index_Properties_Name.GetValueOrNull(propertyName);
//		    if (property != null)
//		    {
//			    return property;
//		    }
//		    if (entityType.BaseType != null)
//		    {
//			    return GetProperty_Reccursive(entityType.BaseType, propertyName);
//		    }
//		    throw new Exception("Could not found the property.");
//	    }

//		internal static void BuildMapping_FixTPH(Z.EntityFramework.Plus.Internal.Core.Mapping.SchemaMapping mapping, Schema conceptual, Schema storage)
//		{
//			conceptual.EntityTypes.Where(x => x.IsTPH && x.BaseType == null).ToList().ForEach(entity =>
//			{
//				// ADD generic scalar mapping
//				entity.GenericEntitySetMapping.MappingFragment.ScalarAccessors
//					//.Where(x => !x.AccessorPath.Contains(".")) // Why complex type was not added before?
//					.Where(x => !entity.EntityTypeMapping.MappingFragment.ScalarAccessors.Exists(y => y.AccessorPath == x.AccessorPath)).ToList().ForEach(x =>
//						entity.EntityTypeMapping.MappingFragment.ScalarAccessors.Add(new ScalarAccessorMapping
//						{
//							IsStorageMapped = true,
//							Type = x.Type,
//							AccessorPath = x.AccessorPath,
//							ColumnName = x.ColumnName,
//							IsComputed = x.IsComputed
//						}));
//			});

//			// FOR EACH TPH entity, ensure to add all column from bass class
//			foreach (var tphEntity in conceptual.EntityTypes.Where(x => x.IsTPH))
//			{
//				var baseEntity = tphEntity.BaseType;

//				while (baseEntity != null)
//				{
//					if (baseEntity.EntityTypeMapping != null)
//					{
//						var baseAccessors = baseEntity.EntityTypeMapping.MappingFragment.ScalarAccessors;

//						foreach (var accessor in baseAccessors)
//						{
//							// CHECK if that has not already be added then add the key
//							if (!tphEntity.EntityTypeMapping.MappingFragment.ScalarAccessors.Exists(x => x.ColumnName == accessor.ColumnName))
//							{
//								tphEntity.EntityTypeMapping.MappingFragment.ScalarAccessors.Add(accessor);
//							}
//						}
//					}

//					baseEntity = baseEntity.BaseType;
//				}
//			}
//		}

//	}
//}

//#endif
//#endif