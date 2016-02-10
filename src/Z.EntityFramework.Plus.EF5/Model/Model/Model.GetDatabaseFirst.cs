// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Data.Entity;
using System.Xml.Linq;
using Z.EntityFramework.Plus.Internal.Core.Infrastructure;
using Z.EntityFramework.Plus.Internal.Core.Mapping;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        /// <summary>
        ///     A DbContext extension method that gets database first model.
        /// </summary>
        /// <param name="context">The @this to act on.</param>
        /// <returns>The database first model.</returns>
        internal static DbModelPlus GetDatabaseFirst(DbContext context)
        {
            var conceptualString = context.GetConceptualModelString();
            var storageString = context.GetStorageModelString();
            var mappingString = context.GetMappingModelString();


            return GetDatabaseFirst(conceptualString, storageString, mappingString);
        }

        public static DbModelPlus GetDatabaseFirst(string conceptualModel, string storageModel, string mappingModel)
        {
            var model = new DbModelPlus();

            var conceptualDoc = XDocument.Parse(conceptualModel);
            var storeageDoc = XDocument.Parse(storageModel);
            var mappingDoc = XDocument.Parse(mappingModel);

            var conceptualRoot = conceptualDoc.Root.RemoveAllNamespaces();
            var storageRoot = storeageDoc.Root.RemoveAllNamespaces();
            var mappingRoot = mappingDoc.Root.RemoveAllNamespaces();

            var conceptualString = conceptualRoot.ToString();
            var storageString = storageRoot.ToString();
            var mappingString = mappingRoot.ToString();

            model.ConceptualModel = conceptualString.DeserializeXml<Schema>();
            model.StoreModel = storageString.DeserializeXml<Schema>();
            model.MappingModel = mappingString.DeserializeXml<SchemaMapping>();

            BuildSchema(model.ConceptualModel);
            BuildSchema(model.StoreModel, true);
            BuildMapping(model.MappingModel, model.ConceptualModel, model.StoreModel);

            return model;
        }
    }
}

#endif