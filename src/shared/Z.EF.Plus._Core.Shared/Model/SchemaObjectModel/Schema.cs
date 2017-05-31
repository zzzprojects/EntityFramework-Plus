// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel
{
    /// <summary>A schema.</summary>
    public class Schema
    {
        /// <summary>Gets or sets the name of the index entity types.</summary>
        /// <value>The name of the index entity types.</value>
        [XmlIgnore]
        internal Dictionary<string, SchemaEntityType> Index_EntityTypes_Name { get; set; }

        [XmlIgnore]
        internal EntityContainer EntityContainer { get; set; }

        #region XmlDeserialization

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>A list of types of the entities.</value>
        [XmlElement("EntityType")]
        public List<SchemaEntityType> EntityTypes { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The entity container.</value>
        [XmlElement("EntityContainer")]
        public List<EntityContainer> EntityContainers { get; set; }

#endregion
    }
}

#endif
#endif