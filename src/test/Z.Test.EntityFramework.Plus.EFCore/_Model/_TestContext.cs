// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class TestContext : DbContext
    {
        public TestContext() : base()
        {
            Database.EnsureCreated();
        }

        public TestContext(DbContextOptions options) : base(options)
        {
			Database.EnsureCreated();
		}

        public TestContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null)
        {
            Database.EnsureCreated();

			//// TODO: Remove this when cast issue will be fixed
			//QueryFilterManager.GlobalFilters.Clear();
			//QueryFilterManager.GlobalInitializeFilterActions.Clear();

			//if (enableFilter1 != null)
   //         {
   //             this.Filter<Inheritance_Interface_Entity>(QueryFilterHelper.Filter.Filter1, entities => entities.Where(x => x.ColumnInt != 1), isEnabled);
   //             if (!isEnabled && enableFilter1.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter1).Enable();
   //             }
   //             else if (isEnabled && !enableFilter1.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter1).Disable();
   //             }
   //         }
   //         if (enableFilter2 != null)
   //         {
   //             this.Filter<Inheritance_Interface_IEntity>(QueryFilterHelper.Filter.Filter2, entities => entities.Where(x => x.ColumnInt != 2), isEnabled);
   //             if (!isEnabled && enableFilter2.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter2).Enable();
   //             }
   //             else if (isEnabled && !enableFilter2.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter2).Disable();
   //             }
   //         }
   //         if (enableFilter3 != null)
   //         {
   //             this.Filter<Inheritance_Interface_Base>(QueryFilterHelper.Filter.Filter3, entities => entities.Where(x => x.ColumnInt != 3), isEnabled);
   //             if (!isEnabled && enableFilter3.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter3).Enable();
   //             }
   //             else if (isEnabled && !enableFilter3.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter3).Disable();
   //             }
   //         }
   //         if (enableFilter4 != null)
   //         {
   //             this.Filter<Inheritance_Interface_IBase>(QueryFilterHelper.Filter.Filter4, entities => entities.Where(x => x.ColumnInt != 4), isEnabled);
   //             if (!isEnabled && enableFilter4.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter4).Enable();
   //             }
   //             else if (isEnabled && !enableFilter4.Value)
   //             {
   //                 this.Filter(QueryFilterHelper.Filter.Filter4).Disable();
   //             }
   //         }

   //         if (excludeClass != null && excludeClass.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter1).Disable(typeof (Inheritance_Interface_Entity));
   //         }

   //         if (excludeInterface != null && excludeInterface.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter2).Disable(typeof (Inheritance_Interface_IEntity));
   //         }

   //         if (excludeBaseClass != null && excludeBaseClass.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter3).Disable(typeof (Inheritance_Interface_Base));
   //         }

   //         if (excludeBaseInterface != null && excludeBaseInterface.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter4).Disable(typeof (Inheritance_Interface_IBase));
   //         }

   //         if (includeClass != null && includeClass.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter1).Enable(typeof (Inheritance_Interface_IEntity));
   //         }

   //         if (includeInterface != null && includeInterface.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter2).Enable(typeof (Inheritance_Interface_IEntity));
   //         }

   //         if (includeBaseClass != null && includeBaseClass.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter3).Enable(typeof (Inheritance_Interface_Base));
   //         }

   //         if (includeBaseInterface != null && includeBaseInterface.Value)
   //         {
   //             this.Filter(QueryFilterHelper.Filter.Filter4).Enable(typeof (Inheritance_Interface_IBase));
   //         }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SqlConnection(My.Config.ConnectionStrings.TestDatabase));

			base.OnConfiguring(optionsBuilder);
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
//			foreach (var entity in modelBuilder.Model.GetEntityTypes())
//			{
//				entity.Relational().TableName = entity.DisplayName();
//			}

//			AuditManager.DefaultConfiguration.AutoSavePreAction = (ApplicationDbContext, audit) =>
//(ApplicationDbContext as TestContext).AuditEntries.AddRange(audit.Entries);

			modelBuilder.Entity<Entity_ManyGuid>().HasKey(guid => new { guid.ID1, guid.ID2, guid.ID3 });

			//tell EF these are our derivative types for TPH
			modelBuilder.Entity<Inheritance_TPH_Cat>();
			modelBuilder.Entity<Inheritance_TPH_Dog>();

			base.OnModelCreating(modelBuilder);
		}

		#region Association

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

        public DbSet<Entity_Guid> Entity_Guids { get; set; }

        public DbSet<Entity_ManyGuid> Entity_ManyGuids { get; set; }

		#endregion

		#region Inheritance

        public DbSet<Inheritance_Interface_Entity> Inheritance_Interface_Entities { get; set; }

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