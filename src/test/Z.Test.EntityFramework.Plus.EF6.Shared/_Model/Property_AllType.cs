// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Z.Test.EntityFramework.Plus
{
    public class Property_AllType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int ColumnInt { get; set; }

        public long? BigIntColumn { get; set; }
        public byte[] BinaryColumn { get; set; }
        public bool? BitColumn { get; set; }
        public string CharColumn { get; set; }
        public DateTime? DateColumn { get; set; }
        public DateTime? DateTimeColumn { get; set; }
        public DateTime? DateTime2Column { get; set; }
        public DateTimeOffset? DateTimeOffsetColumn { get; set; }
        public decimal? DecimalColumn { get; set; }
        public double? FloatColumn { get; set; }
        public byte[] ImageColumn { get; set; }
        public int? IntColumn { get; set; }
        public decimal? MoneyColumn { get; set; }
        public string NCharColumn { get; set; }
        public string NTextColumn { get; set; }
        public decimal? NumericColumn { get; set; }
        public string NVarcharColumn { get; set; }
        public string NVarcharMaxColumn { get; set; }
        public float? RealColumn { get; set; }
        public DateTime? SmallDateTimeColumn { get; set; }
        public short? SmallIntColumn { get; set; }
        public decimal? SmallMoneyColumn { get; set; }
        public string TextColumn { get; set; }
        public byte? TinyIntColumn { get; set; }
        public Guid? UniqueIdentifierColumn { get; set; }
        public byte[] VarBinaryColumn { get; set; }
        public byte[] VarBinaryMaxColumn { get; set; }
        public string VarcharColumn { get; set; }
        public string VarcharMaxColumn { get; set; }
        public string XmlColumn { get; set; }
    }
}