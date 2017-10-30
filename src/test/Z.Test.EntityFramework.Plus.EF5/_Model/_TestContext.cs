// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Z.EntityFramework.Plus;
using System.Data.Entity;

namespace Z.Test.EntityFramework.Plus
{
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

    public partial class TestContext : DbContext
    {
        public TestContext() : base(My.Config.ConnectionStrings.TestDatabase)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TestContext>());
        }

        public TestContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null) : base(My.Config.ConnectionStrings.TestDatabase)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TestContext>());

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

        #region Association

        public DbSet<Association_ManyToMany_Left> Association_ManyToMany_Lefts { get; set; }

        public DbSet<Association_ManyToMany_Right> Association_ManyToMany_Rights { get; set; }

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

        #region Association OneToSingleAndMany

        #endregion

        public DbSet<Association_OneToSingleAndMany_Left> Association_OneToSingleAndMany_Lefts { get; set; }

        public DbSet<Association_OneToSingleAndMany_Right> Association_OneToSingleAndMany_Rights { get; set; }

        public DbSet<Association_OneToSingleAndMany_RightRight> Association_OneToSingleAndMany_RightRights { get; set; }

        public DbSet<Association_OneToSingleAndMany_RightRightRight> Association_OneToSingleAndMany_RightRightRights { get; set; }

        public DbSet<Association_OneToSingleAndMany_RightRightRightRight> Association_OneToSingleAndMany_RightRightRightRights { get; set; }

        #region Audit

        public DbSet<AuditEntry> AuditEntries { get; set; }

        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

        public DbSet<AuditEntry_Extended> AuditEntry_Extendeds { get; set; }

        public DbSet<AuditEntryProperty_Extended> AuditEntryProperty_Extendeds { get; set; }

        #endregion

        #region Entity

        public DbSet<Entity_Basic> Entity_Basics { get; set; }

        public DbSet<Entity_Basic_WithString> Entity_Basic_WithStrings { get; set; }

        public DbSet<ZZZ_Entity_Basic> ZZZ_Entity_Basics { get; set; }

        public DbSet<Entity_Basic_Many> Entity_Basic_Manies { get; set; }

        public DbSet<Internal_Entity_Basic> Internal_Entity_Basics { get; set; }

        public DbSet<Internal_Entity_Basic_Many> Internal_Entity_Basic_Manies { get; set; }

        public DbSet<Entity_Guid> Entity_Guids { get; set; }

        public DbSet<Entity_ManyGuid> Entity_ManyGuids { get; set; }

        public DbSet<Entity_Proxy> Entity_Proxies { get; set; }

        public DbSet<Entity_Proxy_Right> Entity_Proxy_Rights { get; set; }

        public DbSet<Entity_Complex> Entity_Complexes { get; set; }

        public DbSet<Entity_Enum> Entity_Enums { get; set; }

        #endregion

        #region Inheritance

        public DbSet<Inheritance_Interface_Entity> Inheritance_Interface_Entities { get; set; }

        public DbSet<Inheritance_Interface_Entity_LazyLoading> Inheritance_Interface_Entities_LazyLoading { get; set; }

	    public DbSet<Inheritance_TPC_Animal> Inheritance_TPC_Animals { get; set; }

		public DbSet<Inheritance_TPC_Cat> Inheritance_TPC_Cats { get; set; }

	    public DbSet<Inheritance_TPC_Dog> Inheritance_TPC_Dogs { get; set; }

		public DbSet<Inheritance_TPH_Animal> Inheritance_TPH_Animals { get; set; }

        public DbSet<Inheritance_TPT_Animal> Inheritance_TPT_Animals { get; set; }

        public DbSet<Inheritance_TPT_Cat> Inheritance_TPT_Cats { get; set; }

	    public DbSet<Inheritance_TPT_Dog> Inheritance_TPT_Dogs { get; set; }

		public DbSet<Property_AllType> Property_AllTypes { get; set; }

		#endregion
    }
}