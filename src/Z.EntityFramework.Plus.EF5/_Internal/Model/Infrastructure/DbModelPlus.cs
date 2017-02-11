// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using Z.EntityFramework.Plus.Internal.Core.Mapping;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal.Core.Infrastructure
{
    /// <summary>A data Model for the database.</summary>
    internal class DbModelPlus
    {
        /// <summary>Gets or sets the conceptual model.</summary>
        /// <value>The conceptual model.</value>
        public Schema ConceptualModel { get; set; }

        /// <summary>Gets or sets the store model.</summary>
        /// <value>The store model.</value>
        public Schema StoreModel { get; set; }

        /// <summary>Gets or sets the mapping model.</summary>
        /// <value>The mapping model.</value>
        public SchemaMapping MappingModel { get; set; }

        /// <summary>Gets the entity.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A T.</returns>
        public SchemaEntityType<T> Entity<T>() where T : class
        {
var type = typeof (T).Name;

if (!ConceptualModel.Index_EntityTypes_Name.ContainsKey(type))
{
	throw new InvalidOperationException(ExceptionMessage.GeneralException);
}

            var conceptual = ConceptualModel.Index_EntityTypes_Name[type];
            var entityContainer = new SchemaEntityType<T> {Info = conceptual};
            return entityContainer;
        }
    }
}

#endif
#endif