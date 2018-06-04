// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
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
    public class EntityContainerEntitySet
    {
        /// <summary>Gets or sets the type of the entity.</summary>
        /// <value>The type of the entity.</value>
        [XmlIgnore]
        public SchemaEntityType EntityType { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is tpc.</summary>
	    /// <value>true if this object is tpc, false if not.</value>
	    [XmlIgnore]
	    public bool IsTPC { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is tph.</summary>
	    /// <value>true if this object is tph, false if not.</value>
	    [XmlIgnore]
	    public bool IsTPH { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is tpt.</summary>
	    /// <value>true if this object is tpt, false if not.</value>
	    [XmlIgnore]
	    public bool IsTPT { get; set; }

	    /// <summary>Gets or sets the entity set mapping.</summary>
	    /// <value>The entity set mapping.</value>
	    [XmlIgnore]
	    public EntitySetMapping EntitySetMapping { get; set; }

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
        /// <value>The name of the entity type.</value>
        [XmlAttribute("EntityType")]
        public string EntityTypeName { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The schema.</value>
        [XmlAttribute("Schema")]
        public string Schema { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The table.</value>
        [XmlAttribute("Table")]
        public string Table { get; set; }

#endregion
    }
}

#endif
#endif