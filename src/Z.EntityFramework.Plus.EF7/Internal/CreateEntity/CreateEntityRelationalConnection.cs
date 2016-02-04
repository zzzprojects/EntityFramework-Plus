// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.


#if EF7
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;

namespace Z.EntityFramework.Plus
{
    internal class CreateEntityRelationConnection : IRelationalConnection
    {
        public CreateEntityRelationConnection(IRelationalConnection originalRelationalConnection)
        {
            OriginalRelationalConnection = originalRelationalConnection;
        }

        public DbDataReader OriginalDataReader { get; set; }

        private IRelationalConnection OriginalRelationalConnection { get; }

        public int? CommandTimeout
        {
            get { return OriginalRelationalConnection.CommandTimeout; }
            set { OriginalRelationalConnection.CommandTimeout = value; }
        }

        public string ConnectionString
        {
            get { return OriginalRelationalConnection.ConnectionString; }
        }

        public DbConnection DbConnection
        {
            get { return new CreateEntityConnection(OriginalRelationalConnection.DbConnection, OriginalDataReader); }
        }

        public DbTransaction DbTransaction
        {
            get { return OriginalRelationalConnection.DbTransaction; }
        }

        public bool IsMultipleActiveResultSetsEnabled
        {
            get { return OriginalRelationalConnection.IsMultipleActiveResultSetsEnabled; }
        }

        public IRelationalTransaction Transaction
        {
            get { return OriginalRelationalConnection.Transaction; }
        }

        public IRelationalTransaction BeginTransaction()
        {
            return OriginalRelationalConnection.BeginTransaction();
        }

        public IRelationalTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return OriginalRelationalConnection.BeginTransaction(isolationLevel);
        }

        public Task<IRelationalTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return OriginalRelationalConnection.BeginTransactionAsync(cancellationToken);
        }

        public Task<IRelationalTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OriginalRelationalConnection.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public void Close()
        {
            OriginalRelationalConnection.Close();
        }

        public void Dispose()
        {
            OriginalRelationalConnection.Dispose();
        }

        public void Open()
        {
            OriginalRelationalConnection.Open();
        }

        public Task OpenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return OriginalRelationalConnection.OpenAsync(cancellationToken);
        }

        public IRelationalTransaction UseTransaction(DbTransaction transaction)
        {
            return OriginalRelationalConnection.UseTransaction(transaction);
        }
    }
}

#endif