// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EFCORE
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Z.EntityFramework.Plus;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.Test.EntityFramework.Plus
{
#if EF5 || EF6
    public class TestContextInitializer : CreateDatabaseIfNotExists<TestContext>
    {
        protected override void Seed(TestContext context)
        {
            var sql = @"
TRUNCATE TABLE Inheritance_TPC_Cat
IF IDENT_CURRENT( 'Inheritance_TPC_Cat' ) < 1000000
BEGIN
	DBCC CHECKIDENT('Inheritance_TPC_Cat', RESEED, 1000000)
END
";
            using (var connection = new SqlConnection(My.Config.ConnectionStrings.TestDatabase))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            base.Seed(context);
        }
    }
#endif

    public partial class TestContextMemory : DbContext
    {
#if EF5 || EF6
        public TestContext() : base(My.Config.ConnectionStrings.TestDatabase)
#elif EFCORE
        public TestContextMemory()
#endif
        {
#if EF5 || EF6
            Database.SetInitializer(new TestContextInitializer());
#elif EFCORE
            Database.EnsureCreated();
#endif
        }

#if EFCORE
        public TestContextMemory(DbContextOptions options) : base(options)
        {
            
        }
#endif


#if EF5 || EF6
        public TestContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null) : base(My.Config.ConnectionStrings.TestDatabase)
#elif EFCORE
        public TestContextMemory(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null)
#endif
        {
#if EF5 || EF6
            Database.SetInitializer(new CreateDatabaseIfNotExists<TestContext>());
#elif EFCORE
            Database.EnsureCreated();
#endif
#if EFCORE
    // TODO: Remove this when cast issue will be fixed
            QueryFilterManager.GlobalFilters.Clear();
            QueryFilterManager.GlobalInitializeFilterActions.Clear();
#endif

            if (enableFilter1 != null)
            {
                this.Filter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter1, entities => entities.Where(x => x.ColumnInt != 1), isEnabled);
                if (!isEnabled && enableFilter1.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter1).Enable();
                }
                else if (isEnabled && !enableFilter1.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter1).Disable();
                }
            }
            if (enableFilter2 != null)
            {
                this.Filter<Inheritance_Interface_IEntity>(QueryFilterHelper.Filter.Filter2, entities => entities.Where(x => x.ColumnInt != 2), isEnabled);
                if (!isEnabled && enableFilter2.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter2).Enable();
                }
                else if (isEnabled && !enableFilter2.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter2).Disable();
                }
            }
            if (enableFilter3 != null)
            {
                this.Filter<Inheritance_Interface_Base>(QueryFilterHelper.Filter.Filter3, entities => entities.Where(x => x.ColumnInt != 3), isEnabled);
                if (!isEnabled && enableFilter3.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter3).Enable();
                }
                else if (isEnabled && !enableFilter3.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter3).Disable();
                }
            }
            if (enableFilter4 != null)
            {
                this.Filter<Inheritance_Interface_IBase>(QueryFilterHelper.Filter.Filter4, entities => entities.Where(x => x.ColumnInt != 4), isEnabled);
                if (!isEnabled && enableFilter4.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter4).Enable();
                }
                else if (isEnabled && !enableFilter4.Value)
                {
                    this.Filter(QueryFilterHelper.Filter.Filter4).Disable();
                }
            }

            if (excludeClass != null && excludeClass.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter1).Disable(typeof (Inheritance_Interface_Entity));
            }

            if (excludeInterface != null && excludeInterface.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter2).Disable(typeof (Inheritance_Interface_IEntity));
            }

            if (excludeBaseClass != null && excludeBaseClass.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter3).Disable(typeof (Inheritance_Interface_Base));
            }

            if (excludeBaseInterface != null && excludeBaseInterface.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter4).Disable(typeof (Inheritance_Interface_IBase));
            }

            if (includeClass != null && includeClass.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter1).Enable(typeof (Inheritance_Interface_IEntity));
            }

            if (includeInterface != null && includeInterface.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter2).Enable(typeof (Inheritance_Interface_IEntity));
            }

            if (includeBaseClass != null && includeBaseClass.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter3).Enable(typeof (Inheritance_Interface_Base));
            }

            if (includeBaseInterface != null && includeBaseInterface.Value)
            {
                this.Filter(QueryFilterHelper.Filter.Filter4).Enable(typeof (Inheritance_Interface_IBase));
            }
        }

#if EF5 || EF6
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Association
            {
                // Many to Many
                {
                    modelBuilder.Entity<Association_ManyToMany_Left>()
                        .HasMany(x => x.Rights)
                        .WithMany(x => x.Lefts)
                        .Map(c =>
                        {
                            c.MapLeftKey("ChildID");
                            c.MapRightKey("ParentID");
                            c.ToTable("Association_ManyToMany");
                        });
                }

                // Many
                {
                    modelBuilder.Entity<Association_OneToMany_Left>()
                        .HasMany(x => x.Rights)
                        .WithRequired(x => x.Left);
                }
            }

            // Association Multi
            {
                // Many
                {
                    modelBuilder.Entity<Association_Multi_OneToMany_Left>()
                        .HasMany(x => x.Right1s)
                        .WithRequired(x => x.Left);

                    modelBuilder.Entity<Association_Multi_OneToMany_Left>()
                        .HasMany(x => x.Right2s)
                        .WithRequired(x => x.Left);

                    modelBuilder.Entity<Association_OneToManyToMany_Left>()
                        .HasMany(x => x.Rights)
                        .WithRequired(x => x.Left);

                    modelBuilder.Entity<Association_OneToManyToMany_Right>()
                        .HasMany(x => x.Rights)
                        .WithRequired(x => x.Left);
                }
            }

            // Entity
            {
                modelBuilder.ComplexType<Entity_Complex_Info>();
                modelBuilder.ComplexType<Entity_Complex_Info_Info>();
            }

            // Inheritance
            {
                modelBuilder.Entity<Inheritance_TPC_Cat>().Map(m =>
                {
                    m.MapInheritedProperties();
                    m.ToTable("Inheritance_TPC_Cat");
                });

                modelBuilder.Entity<Inheritance_TPC_Dog>().Map(m =>
                {
                    m.MapInheritedProperties();
                    m.ToTable("Inheritance_TPC_Dog");
                });
            }

            // Many
            {
                modelBuilder.Entity<AuditEntry>().HasMany(x => x.Properties).WithRequired(x => x.Parent);
            }

            modelBuilder.Configurations.Add(new Internal_Entity_Basic.EntityPropertyVisibilityConfiguration());
            modelBuilder.Configurations.Add(new Internal_Entity_Basic_Many.EntityPropertyVisibilityConfiguration());
        }
#elif EFCORE
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(new SqlConnection(My.Config.ConnectionStrings.TestDatabase));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }

            AuditManager.DefaultConfiguration.AutoSavePreAction = (ApplicationDbContext, audit) =>
(ApplicationDbContext as TestContext).AuditEntries.AddRange(audit.Entries);

            modelBuilder.Entity<Entity_ManyGuid>().HasKey(guid => new {guid.ID1, guid.ID2, guid.ID3});

            // Association
            {
            }

            // Audit
            {
                //modelBuilder.Entity<AuditEntry>().HasKey(x => x.AuditEntryID);
                //modelBuilder.Entity<AuditEntryProperty>().Ignore(x => x.NewValue);
                //modelBuilder.Entity<AuditEntryProperty>().Ignore(x => x.OldValue);
                //modelBuilder.Entity<AuditEntry>().HasMany(x => x.Properties).WithOne(x => x.AuditEntry);
                //modelBuilder.Entity<AuditEntryProperty>().HasKey(x => x.AuditEntryPropertyID);

            }
        }

#endif

        #region Association

#if EF5 || EF6
        public DbSet<Association_ManyToMany_Left> Association_ManyToMany_Lefts { get; set; }

        public DbSet<Association_ManyToMany_Right> Association_ManyToMany_Rights { get; set; }

#endif
        public DbSet<Association_OneToMany_Left> Association_OneToMany_Lefts { get; set; }

        public DbSet<Association_OneToMany_Right> Association_OneToMany_Rights { get; set; }

        #endregion

        #region Association Multi

        public DbSet<Association_Multi_OneToMany_Left> Association_Multi_OneToMany_Lefts { get; set; }

        public DbSet<Association_Multi_OneToMany_Right1> Association_Multi_OneToMany_Right1s { get; set; }

        public DbSet<Association_Multi_OneToMany_Right2> Association_Multi_OneToMany_Right2s { get; set; }

        #endregion

        #region Association ToManyToMany

        public DbSet<Association_OneToManyToMany_Left> Association_Multi_OneToManyToMany_Lefts { get; set; }

        public DbSet<Association_OneToManyToMany_Right> Association_Multi_OneToManyToMany_Rights { get; set; }

        public DbSet<Association_OneToManyToMany_RightRight> Association_Multi_OneToManyToMany_RightRights { get; set; }

        #endregion

        #region Audit

        public DbSet<AuditEntry> AuditEntries { get; set; }

        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

        #endregion

        #region Entity

        public DbSet<Entity_Basic> Entity_Basics { get; set; }

        public DbSet<ZZZ_Entity_Basic> ZZZ_Entity_Basics { get; set; }

        public DbSet<Entity_Basic_Many> Entity_Basic_Manies { get; set; }

#if EF5 || EF6
        public DbSet<Internal_Entity_Basic> Internal_Entity_Basics { get; set; }

        public DbSet<Internal_Entity_Basic_Many> Internal_Entity_Basic_Manies { get; set; }
#endif


        public DbSet<Entity_Guid> Entity_Guids { get; set; }

        public DbSet<Entity_ManyGuid> Entity_ManyGuids { get; set; }

#if EF5 || EF6
        public DbSet<Entity_Complex> Entity_Complexes { get; set; }
#endif

#endregion

#region Inheritance

        public DbSet<Inheritance_Interface_Entity> Inheritance_Interface_Entities { get; set; }

#if EF5 || EF6
        public DbSet<Inheritance_TPC_Animal> Inheritance_TPC_Animals { get; set; }

        public DbSet<Inheritance_TPH_Animal> Inheritance_TPH_Animals { get; set; }

        public DbSet<Inheritance_TPT_Animal> Inheritance_TPT_Animals { get; set; }

#endif
        public DbSet<Inheritance_TPT_Cat> Inheritance_TPT_Cats { get; set; }

        public DbSet<Property_AllType> Property_AllTypes { get; set; }

#endregion
    }
}
#endif