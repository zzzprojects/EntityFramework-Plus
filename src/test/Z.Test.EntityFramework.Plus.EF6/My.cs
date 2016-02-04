// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Configuration;
using System.Data.SqlClient;

namespace Z.Test.EntityFramework.Plus
{
    public class My
    {
        static My()
        {
#if EF5 || EF6
            var sql = @"
TRUNCATE TABLE Inheritance_TPC_Cat
IF IDENT_CURRENT( 'Inheritance_TPC_Cat' ) < 1000000
BEGIN
	DBCC CHECKIDENT('Inheritance_TPC_Cat', RESEED, 1000000)
END
";
            using (var connection = new SqlConnection(Config.ConnectionStrings.TestDatabase))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }

#endif
        }

        public class Config
        {
            public class ConnectionStrings
            {
                public static string TestDatabase = ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString;
            }
        }
    }
}