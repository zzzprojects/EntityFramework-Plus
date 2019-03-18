//#if FULL || BATCH_DELETE || BATCH_UPDATE
//#if EF5 || EF6
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Z.EF.Plus.BatchUpdate.Shared.Model;
//using Z.EntityFramework.Plus;
//using Z.EntityFramework.Plus.Internal.Core.Infrastructure;
//using Z.EntityFramework.Plus.Internal.Core.Mapping;
//using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;
//using Z.EF.Plus.BatchUpdate.Shared.Model;
//using Z.EntityFramework.Plus;
//using Z.EntityFramework.Plus.Internal.Core.Infrastructure;
//using Z.EntityFramework.Plus.Internal.Core.Mapping;
//using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

//namespace Z.EF.Plus._Core.Shared.Extensions
//{

//    public static class DbModelPlusExtentions
//    {
//	    public static IEnumerable<TableDefinition> GetTableDefinitions<T>(this DbModelPlus model)
//	    {
//		    string strName = typeof(T).Name;

//			//get our table definitions
//		    IEnumerable<TableDefinition> lsTableDefinitions = GetTableDefinitions(model, strName);

//			//holds our new order which everything will be presented
//		    int intCurrentOrder = 0;

//			//this will assist with building our new list of combined definitions
//			Dictionary<string, TableDefinition> dicCombinedTableDefinitions = new Dictionary<string, TableDefinition>();

//			//loop through our table definitions and combine them
//		    foreach (TableDefinition tdTableDefinition in lsTableDefinitions)
//		    {
//			    string strKey = tdTableDefinition.Schema + "-" + tdTableDefinition.Table;

//				//index or retrieve depending if we've hit it before
//				if (!dicCombinedTableDefinitions.ContainsKey(strKey))
//				{
//					//change the order
//					tdTableDefinition.Order = intCurrentOrder;

//					//index
//					dicCombinedTableDefinitions[strKey] = tdTableDefinition;

//					//increment
//					intCurrentOrder++;
//				}
//				else
//				{
//					TableDefinition tdCurrent = dicCombinedTableDefinitions[strKey];

//					//merge our keys
//					tdCurrent.Keys = tdCurrent
//						.Keys
//						.Union(tdTableDefinition.Keys)
//						.GroupBy(i => i.Name)
//						.Select(i => i.FirstOrDefault())
//						.Where(i => i != null)
//						.ToList();

//					//merge our properties
//					tdCurrent.Properties = tdCurrent
//						.Properties
//						.Union(tdTableDefinition.Properties)
//						.GroupBy(i => i.Name)
//						.Select(i => i.FirstOrDefault())
//						.Where(i => i != null)
//						.ToList();
//				}
//			}

//		    return dicCombinedTableDefinitions.Values;
//	    }

//	    private static IEnumerable<TableDefinition> GetTableDefinitions(DbModelPlus model, string strName)
//	    {
//		    //get our entity type
//		    SchemaEntityType setEntityType = model.ConceptualModel.Index_EntityTypes_Name[strName];

//			//create our update entity
//		    TableDefinition ueEntity = new TableDefinition();

//		    //create our return list and add our definition
//		    List<TableDefinition> lsTableDefinitions = new List<TableDefinition>();
		    
//			//if we have mapping data, leverage it
//			if (setEntityType.EntityTypeMapping != null)
//		    {
//			    lsTableDefinitions.Add(ueEntity);

//				ueEntity.Schema = setEntityType.EntityTypeMapping.MappingFragment.StoreEntitySet.Schema;
//			    ueEntity.Table = setEntityType.EntityTypeMapping.MappingFragment.StoreEntitySet.Table;

//			    //map our keys for this model
//			    List<ScalarPropertyMapping> lsKeys = new List<ScalarPropertyMapping>();
//			    foreach (PropertyRefElement propertyKey in setEntityType.EntityTypeMapping.MappingFragment.StoreEntitySet.EntityType.Key.PropertyRefs)
//			    {
//				    ScalarPropertyMapping mappingProperty = setEntityType.EntityTypeMapping.MappingFragment.ScalarProperties.Find(x => x.Name == propertyKey.Name);

//				    if (mappingProperty == null)
//				    {
//					    throw new Exception(string.Format(ExceptionMessage.BatchOperations_PropertyNotFound, propertyKey.Name));
//				    }

//				    lsKeys.Add(mappingProperty);
//			    }
//			    ueEntity.Keys = lsKeys;

//			    //clone our list of properties
//			    ueEntity.Properties = setEntityType.EntityTypeMapping.MappingFragment.ScalarProperties.ToList();

//			    //if there's no base type, stop here
//			    if (string.IsNullOrEmpty(setEntityType.BaseType))
//			    {
//				    return lsTableDefinitions;
//			    }
//			}

//			//stop here if there's no base type
//		    if (string.IsNullOrEmpty(setEntityType.BaseType))
//		    {
//			    return lsTableDefinitions;
//		    }

//		    //strip the alias bit
//		    string strBaseType = setEntityType.BaseType.Replace(model.ConceptualModel.Alias + ".", "");

//		    //get the columns for our base class
//		    IEnumerable<TableDefinition> lsBaseTableDefinitions = GetTableDefinitions(model, strBaseType);

//		    //increment the order of the base column entities by one
//		    foreach (TableDefinition ueBaseEntity in lsBaseTableDefinitions)
//		    {
//			    ueBaseEntity.Order += 1;

//			    lsTableDefinitions.Add(ueBaseEntity);
//		    }

//		    return lsTableDefinitions;
//	    }
//	}
//}
//}
//#endif
//#endif