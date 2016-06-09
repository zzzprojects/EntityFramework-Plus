// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_FUTURE
#if EFCORE
using System.Data;
using System.Data.Common;

namespace Z.EntityFramework.Plus
{
    internal class CreateEntityConnection : DbConnection
    {
        public CreateEntityConnection(DbConnection originalConnection, DbDataReader originalDataReader)
        {
            OriginalConnection = originalConnection;
            OriginalDataReader = originalDataReader;
        }

        private DbConnection OriginalConnection { get; }

        private DbDataReader OriginalDataReader { get; }

        public override string ConnectionString
        {
            get { return OriginalConnection.ConnectionString; }
            set { OriginalConnection.ConnectionString = value; }
        }

        public override string Database
        {
            get { return OriginalConnection.Database; }
        }

        public override string DataSource
        {
            get { return OriginalConnection.DataSource; }
        }

        public override string ServerVersion
        {
            get { return OriginalConnection.ServerVersion; }
        }

        public override ConnectionState State
        {
            get { return OriginalConnection.State; }
        }

        public override void ChangeDatabase(string databaseName)
        {
            OriginalConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            OriginalConnection.Close();
        }

        public override void Open()
        {
            OriginalConnection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return OriginalConnection.BeginTransaction();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new CreateEntityCommand(OriginalConnection.CreateCommand(), OriginalDataReader);
        }
    }
}

#endif
#endif