#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Collections.Generic;
using System.Xml.Serialization;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus.Internal.Core.Mapping
{
    /// <summary>
    ///     Please visit the
    ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
    ///         Microsoft documentation
    ///     </see>
    ///     for more detail.
    /// </summary>
    public class ComplexPropertyMapping
    {
        /// <summary>Gets or sets the type of the complex entity.</summary>
        /// <value>The type of the complex entity.</value>
        [XmlIgnore]
        SchemaEntityType ComplexEntityType { get; set; }

#region XmlDeserialization

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The name of the type.</value>
        [XmlAttribute("TypeName")]
        public string TypeName { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The scalar properties.</value>
        [XmlElement("ScalarProperty")]
        public List<ScalarPropertyMapping> ScalarProperties { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The complex properties.</value>
        [XmlElement("ComplexProperty")]
        public List<ComplexPropertyMapping> ComplexProperties { get; set; }

#endregion
    }
}
#endif
#endif