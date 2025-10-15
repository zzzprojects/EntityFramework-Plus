// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public static class AuditHelper
    {
        public static Audit AutoSaveAudit()
        {
            var audit = new Audit();
            audit.Configuration.AutoSavePreAction = (context, audit1) => (context as TestContext).AuditEntries.AddRange(audit1.Entries);
            return audit;
        }

        public static Audit AutoSaveWithAuditEntryFactory()
        {
            var audit = new Audit();
            audit.Configuration.AutoSavePreAction = (context, audit1) =>
            {
                (context as TestContext).AuditEntry_Extendeds.AddRange(audit1.Entries.Cast<AuditEntry_Extended>());
            };

            audit.Configuration.AuditEntryFactory = args =>
            {
                return new AuditEntry_Extended
                {
                    CreatedBy = "CustomCreatedBy",
                    CreatedDate = new DateTime(1981, 04, 13),
                    EntitySetName = "CustomEntitySetName",
                    EntityTypeName = "CustomEntityTypeName",
                    ExtendedValue = "CustomExtendedValue"
                };
            };

            return audit;
        }

        public static Audit AutoSaveWithAuditEntryPropertyFactory()
        {
            var audit = new Audit();
            audit.CreatedBy = "ZZZ Projects";
            audit.Configuration.AutoSavePreAction = (context, audit1) => (context as TestContext).AuditEntries.AddRange(audit1.Entries);

            audit.Configuration.AuditEntryPropertyFactory = args =>
            {
                return new AuditEntryProperty_Extended
                {
                    RelationName = "CustomRelationName",
                    PropertyName = "CustomPropertyName",
                    ExtendedValue = "CustomExtendedValue"
                };
            };

            return audit;
        }
    }
}