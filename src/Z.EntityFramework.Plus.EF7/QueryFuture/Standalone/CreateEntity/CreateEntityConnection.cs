// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE && EF7
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