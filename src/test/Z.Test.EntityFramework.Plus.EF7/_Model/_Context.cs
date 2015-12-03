// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Data.Entity;

namespace Z.Test.EntityFramework.Plus
{
    public class EntityContext : DbContext
    {
        public EntityContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<EntitySimple> EntitySimples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SqlConnection(ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString));
        }
    }
}