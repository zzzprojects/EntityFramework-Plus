// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.Xml.Serialization;

namespace Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel
{
    /// <summary>
    ///     Please visit the
    ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
    ///         Microsoft documentation
    ///     </see>
    ///     for more detail.
    /// </summary>
    public class Property
    {
	    /// <summary>Gets or sets the parent entity.</summary>
	    /// <value>The parent entity.</value>
	    [XmlIgnore]
	    public SchemaEntityType ParentEntity { get; set; }

	    /// <summary>Gets or sets the is concurrency.</summary>
	    /// <value>The is concurrency.</value>
	    [XmlIgnore]
	    public bool IsConcurrency { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is complex.</summary>
	    /// <value>true if this object is complex, false if not.</value>
	    [XmlIgnore]
	    public bool IsComplex { get; set; }

	    /// <summary>Gets or sets the type of the complex.</summary>
	    /// <value>The type of the complex.</value>
	    [XmlIgnore]
	    public SchemaEntityType ComplexType { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is enum.</summary>
	    /// <value>true if this object is enum, false if not.</value>
	    [XmlIgnore]
	    public bool IsEnum { get; set; }

	    /// <summary>Gets or sets the type of the enum.</summary>
	    /// <value>The type of the enum.</value>
	    [XmlIgnore]
	    public SchemaEnumType EnumType { get; set; }

	    /// <summary>Gets or sets the type.</summary>
	    /// <value>The type.</value>
	    [XmlIgnore]
	    public Type Type { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is primary key.</summary>
	    /// <value>true if this object is primary key, false if not.</value>
	    [XmlIgnore]
	    public bool IsPrimaryKey { get; set; }

	    /// <summary>Gets or sets a value indicating whether this object is computed.</summary>
	    /// <value>true if this object is computed, false if not.</value>
	    [XmlIgnore]
	    public bool IsComputed { get; set; }

		#region XmlDeserialization

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute]
        public string Name { get; set; }

	    /// <summary>
	    ///     Please visit the
	    ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
	    ///         Microsoft documentation
	    ///     </see>
	    ///     for more detail.
	    /// </summary>
	    /// <value>The name of the type.</value>
	    [XmlAttribute("Type")]
	    public string TypeName { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>true if nullable, false if not.</value>
		[XmlAttribute("Nullable")]
		public bool Nullable { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The store generated pattern.</value>
		[XmlAttribute("StoreGeneratedPattern")]
		public string StoreGeneratedPattern { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>true if unicode, false if not.</value>
		[XmlAttribute("Unicode")]
		public bool Unicode { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>true if fixed length, false if not.</value>
		[XmlAttribute("FixedLength")]
		public bool FixedLength { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The length of the maximum.</value>
		[XmlAttribute("MaxLength")]
		public string MaxLength { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The precision.</value>
		[XmlAttribute("Precision")]
		public int Precision { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The scale.</value>
		[XmlAttribute("Scale")]
		public int Scale { get; set; }

		/// <summary>
		///     Please visit the
		///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399292(v=vs.100).aspx">
		///         Microsoft documentation
		///     </see>
		///     for more detail.
		/// </summary>
		/// <value>The concurrency mode.</value>
		[XmlAttribute("ConcurrencyMode")]
		public string ConcurrencyMode { get; set; }

		#endregion
	}
}

#endif
#endif