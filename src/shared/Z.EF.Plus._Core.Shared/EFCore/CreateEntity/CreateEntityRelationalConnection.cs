// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_FUTURE
#if EFCORE
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

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

        public bool IsMultipleActiveResultSetsEnabled
        {
            get { return OriginalRelationalConnection.IsMultipleActiveResultSetsEnabled; }
        }

        public IDbContextTransaction CurrentTransaction
        {
            get { return OriginalRelationalConnection.CurrentTransaction; }
        }

        public IValueBufferCursor ActiveCursor
        {
            get { return OriginalRelationalConnection.ActiveCursor; }
            set { OriginalRelationalConnection.ActiveCursor = value; }
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

        IDbContextTransaction IRelationalTransactionManager.BeginTransaction(IsolationLevel isolationLevel)
        {
            return OriginalRelationalConnection.BeginTransaction(isolationLevel);
        }

        Task<IDbContextTransaction> IRelationalTransactionManager.BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            return OriginalRelationalConnection.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        IDbContextTransaction IRelationalTransactionManager.UseTransaction(DbTransaction transaction)
        {
            return OriginalRelationalConnection.UseTransaction(transaction);
        }

        IDbContextTransaction IDbContextTransactionManager.BeginTransaction()
        {
            return OriginalRelationalConnection.BeginTransaction();
        }

        Task<IDbContextTransaction> IDbContextTransactionManager.BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return OriginalRelationalConnection.BeginTransactionAsync(cancellationToken);
        }

        public void CommitTransaction()
        {
            OriginalRelationalConnection.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            OriginalRelationalConnection.RollbackTransaction();
        }
    }
}

#endif
#endif