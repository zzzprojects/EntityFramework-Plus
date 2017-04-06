// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Data.Entity;
using System.Linq;
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
        internal static DbModelPlus GetCodeFirstModel(DbContext context)
        {
            var xml = context.GetModelXDocument();
            return GetCodeFirstModel(xml);
        }

        public static DbModelPlus GetCodeFirstModel(XDocument xml)
        {
            var model = new DbModelPlus();

            var root = xml.Root.RemoveAllNamespaces();
            var conceptualString = root.Element("Runtime").Element("ConceptualModels").Elements().ElementAt(0).ToString();
            var storageString = root.Element("Runtime").Element("StorageModels").Elements().ElementAt(0).ToString();
            var mappingString = root.Element("Runtime").Element("Mappings").Elements().ElementAt(0).ToString();

            model.ConceptualModel = conceptualString.DeserializeXml<Schema>();
            model.StoreModel = storageString.DeserializeXml<Schema>();
            model.MappingModel = mappingString.DeserializeXml<SchemaMapping>();

            model.ConceptualModel.EntityContainer = model.ConceptualModel.EntityContainers[0];
            model.StoreModel.EntityContainer = model.StoreModel.EntityContainers[0];
            model.MappingModel.EntityContainerMapping = model.MappingModel.EntityContainerMappings[0];

            BuildSchema(model.ConceptualModel);
            BuildSchema(model.StoreModel, true);
            BuildMapping(model.MappingModel, model.ConceptualModel, model.StoreModel);

            return model;
        }
    }
}

#endif
#endif