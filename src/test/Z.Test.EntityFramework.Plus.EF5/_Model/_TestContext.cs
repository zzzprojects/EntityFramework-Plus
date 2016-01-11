// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using Z.EntityFramework.Plus;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Data.Entity;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class TestContext : DbContext
    {
#if EF5 || EF6
        public TestContext() : base("TestDatabase")
#elif EF7
        public TestContext()
#endif
        {
#if EF7
            Database.EnsureCreated();
#endif
        }


#if EF5 || EF6
        public TestContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null) : base("TestDatabase")
#elif EF7
        public TestContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null)
#endif
        {
#if EF7
            Database.EnsureCreated();
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
        }
#elif EF7
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SqlConnection(ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

        #region Audit

#if EF5 || EF6

        public DbSet<AuditEntry> AuditEntries { get; set; }

        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

#endif

        #endregion

        #region Entity

        public DbSet<Entity_Basic> Entity_Basics { get; set; }

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

        #endregion
    }
}