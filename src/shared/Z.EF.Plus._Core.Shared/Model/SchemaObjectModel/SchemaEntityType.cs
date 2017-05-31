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
using Z.EntityFramework.Plus.Internal.Core.Mapping;

namespace Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel
{
    /// <summary>
    ///     Please visit the
    ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
    ///         Microsoft documentation
    ///     </see>
    ///     for more detail.
    /// </summary>
    public class SchemaEntityType
    {
        /// <summary>Gets or sets the name of the index properties.</summary>
        /// <value>The name of the index properties.</value>
        [XmlIgnore]
        internal Dictionary<string, Property> Index_Properties_Name { get; set; }

        /// <summary>Gets or sets the entity type mapping.</summary>
        /// <value>The entity type mapping.</value>
        [XmlIgnore]
        public EntityTypeMapping EntityTypeMapping { get; set; }

        /// <summary>Gets or sets the set the entity belongs to.</summary>
        /// <value>The entity set.</value>
        [XmlIgnore]
        public EntityContainerEntitySet EntitySet { get; set; }

        /// <summary>Gets or sets the generic entity set mapping.</summary>
        /// <value>The generic entity set mapping.</value>
        [XmlIgnore]
        public EntityTypeMapping GenericEntitySetMapping { get; set; }

#region XmlDeserialization

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The key.</value>
        [XmlElement("Key")]
        public EntityKeyElement Key { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The properties.</value>
        [XmlElement("Property")]
        public List<Property> Properties { get; set; }

#endregion
    }
}

#endif
#endif