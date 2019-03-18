#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
	using System.Xml.Serialization;

namespace Z.EntityFramework.Plus.Internal.Core.Mapping
{
    /// <summary>A condition property mapping.</summary>
    public class ConditionPropertyMapping
    {
#region XmlDeserialization

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The value.</value>
        [XmlAttribute("Value")]
        public string Value { get; set; }

        /// <summary>
        ///     Please visit the
        ///     <see href="http://msdn.microsoft.com/en-us/library/vstudio/bb399202(v=vs.100).aspx">
        ///         Microsoft documentation
        ///     </see>
        ///     for more detail.
        /// </summary>
        /// <value>The name of the column.</value>
        [XmlAttribute("ColumnName")]
        public string ColumnName { get; set; }

#endregion
    }
}
#endif
#endif