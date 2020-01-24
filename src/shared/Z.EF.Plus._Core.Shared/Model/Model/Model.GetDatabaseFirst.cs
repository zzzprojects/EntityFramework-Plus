// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
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
            var modelSplit = "---zzz_multi_model_split_zzz---";

            List<string> modelNames = new List<string>();


			try
            {
	            modelNames = context.GetModelNames();
            }
            catch
            { 
			}

            if (modelNames.Count == 0)
            {
                // Oops, something is wrong! Use the old way instead
                modelNames.Add(context.GetModelName());
            }

            var conceptualString = context.GetConceptualModelString(modelNames, modelSplit);
            var storageString = context.GetStorageModelString(modelNames, modelSplit);
            var mappingString = context.GetMappingModelString(modelNames, modelSplit);


            return GetDatabaseFirst(conceptualString, storageString, mappingString, modelSplit);
        }

        public static DbModelPlus GetDatabaseFirst(ObjectContext context) {

            var modelSplit = "---zzz_multi_model_split_zzz---";
            var asm = context.GetType().Assembly;

            if(!asm.GetManifestResourceNames().Any(x => x.EndsWith(".ssdl"))) {
                asm = context.GetType().BaseType.Assembly;
            }

            var rn = asm.GetManifestResourceNames();
            var ssdl = rn.FirstOrDefault(x => x.EndsWith(".ssdl"));
            var csdl = rn.FirstOrDefault(x => x.EndsWith(".csdl"));
            var msl = rn.FirstOrDefault(x => x.EndsWith(".msl"));

                

            return GetDatabaseFirst(GetXml(asm,csdl), GetXml(asm,ssdl), GetXml(asm,msl), modelSplit);
        }

        private static string GetXml(Assembly asm, string rn) {
            using (var sr = new StreamReader(asm.GetManifestResourceStream(rn))) {
                return sr.ReadToEnd();
            }
        }


        public static DbModelPlus GetDatabaseFirst(string conceptualModel, string storageModel, string mappingModel, string modelSplit)
        {
            var model = new DbModelPlus();

            XDocument conceptualDoc = null;
            XDocument storageDoc = null;
            XDocument mappingDoc = null;

            // Conceptual
            {
                var docs = conceptualModel.Split(new[] { modelSplit }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var doc in docs)
                {
                    if (conceptualDoc == null)
                    {
                        conceptualDoc = XDocument.Parse(doc.Trim());
                    }
                    else
                    {
                        conceptualDoc.Root.Add(XDocument.Parse(doc.Trim()).Root.Elements());
                    }
                }
            }

            // Storage
            {
                var docs = storageModel.Split(new[] { modelSplit }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var doc in docs)
                {
                    if (storageDoc == null)
                    {
                        storageDoc = XDocument.Parse(doc.Trim());
                    }
                    else
                    {
                        storageDoc.Root.Add(XDocument.Parse(doc.Trim()).Root.Elements());
                    }
                }
            }

            // Mapping
            {
                var docs = mappingModel.Split(new[] { modelSplit }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var doc in docs)
                {
                    if (mappingDoc == null)
                    {
                        mappingDoc = XDocument.Parse(doc.Trim());
                    }
                    else
                    {
                        mappingDoc.Root.Add(XDocument.Parse(doc.Trim()).Root.Elements());
                    }
                }
            }

            var conceptualRoot = conceptualDoc.Root.RemoveAllNamespaces();
            var storageRoot = storageDoc.Root.RemoveAllNamespaces();
            var mappingRoot = mappingDoc.Root.RemoveAllNamespaces();

            var conceptualString = conceptualRoot.ToString();
            var storageString = storageRoot.ToString();
            var mappingString = mappingRoot.ToString();

            model.ConceptualModel = conceptualString.DeserializeXml<Schema>();
            model.StoreModel = storageString.DeserializeXml<Schema>();
            model.MappingModel = mappingString.DeserializeXml<SchemaMapping>();

            // FIX multi model
            {
                model.ConceptualModel.EntityContainer = model.ConceptualModel.EntityContainers[0];
                model.StoreModel.EntityContainer = model.StoreModel.EntityContainers[0];
                model.MappingModel.EntityContainerMapping = model.MappingModel.EntityContainerMappings[0];

                model.ConceptualModel.EntityContainer.EntitySets.AddRange(model.ConceptualModel.EntityContainers.Skip(1).SelectMany(x => x.EntitySets));
                model.StoreModel.EntityContainer.EntitySets.AddRange(model.StoreModel.EntityContainers.Skip(1).SelectMany(x => x.EntitySets));
                model.MappingModel.EntityContainerMapping.EntitySetMappings.AddRange(model.MappingModel.EntityContainerMappings.Skip(1).SelectMany(x => x.EntitySetMappings));
            }

            BuildSchema(model.ConceptualModel);
            BuildSchema(model.StoreModel, true);
            BuildMapping(model.MappingModel, model.ConceptualModel, model.StoreModel);

            return model;
        }
    }
}

#endif
#endif
