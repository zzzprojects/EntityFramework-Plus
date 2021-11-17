using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
	public class ModelAndContext
	{
		public class My
		{
			// [REPLACE] is in Beta.
			public static string ConnectionString =
				("Server=[REPLACE];Initial Catalog = [BD]; Integrated Security = true; Connection Timeout = 300; Persist Security Info=True").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", "TestEF_PLUS");

			public static void DeleteBD(DbContext context)
			{
				context.Database.Delete(); 
			}

			public static void CreateBD(DbContext context)
			{
				try
				{
					context.Database.CreateIfNotExists();
					if (!context.Database.CompatibleWithModel(throwIfNoMetadata: true))
					{
						throw new Exception("Delete and Create DataBase");
					}
				}
				catch
				{
					My.DeleteBD(context);
					context.Database.CreateIfNotExists();
				}
			}
		} 

        public class EntitySimple
		{
			public int Id { get; set; }
			public int ColumInt { get; set; }
			public int? ColumIntNullable { get; set; }
			public string ColumString { get; set; }
		}

		[AuditDisplay("EntitySimple_Patate")]
		public class EntitySimpleWithDisplay
		{
			public int Id { get; set; }
			public int ColumInt { get; set; }
			public string ColumString { get; set; }
		}

		public class EntitySimpleWithChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public bool IsActive { get; set; }

			public EntitySimpleChild EntitySimpleChild { get; set; }
		}
		public class EntitySimpleChild
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
			public string ColumnString { get; set; }
			public bool IsActive { get; set; }
		}

		public class EntityContext : DbContext
		{
			public EntityContext() : base(My.ConnectionString)
			{
			} 
			// begin BatchDelete_ChangeSet8127_Issue3109
			public DbSet<Customer> Customers { get; set; }
			public DbSet<Order> Orders { get; set; }
			public DbSet<Company> Companies { get; set; }
			// End BatchDelete_ChangeSet8127_Issue3109

			public DbSet<EntitySimple> EntitySimples { get; set; }
			public DbSet<EntitySimpleWithChild> EntitySimpleWithChilds { get; set; }
			public DbSet<EntitySimpleChild> EntitySimpleChilds { get; set; }

			// for BatchUpdateDelete_TPH
			public DbSet<MobileContract> MobileContracts { get; set; }
		    public DbSet<TvContract> TvContracts { get; set; }
		    public DbSet<BroadbandContract> BroadbandContracts { get; set; }
		    public DbSet<Contract> Contracts { get; set; }
            public DbSet<ContractComplex> ContractComplexs { get; set; }
			public DbSet<EntitySimpleWithDisplay> EntitySimpleWithDisplays { get; set; }

			public DbSet<AuditEntry> AuditEntries { get; set; }
			public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
			    // for BatchUpdateDelete_TPH
                modelBuilder.Entity<Contract>().ToTable("Contracts");
			    modelBuilder.Entity<Contract>()
                    .Property(f => f.StartDate)
			        .HasColumnType("datetime2");

				modelBuilder.Entity<UpdatedEntity>()
					.HasKey(x => x.UpdatedEntityId);
				modelBuilder.Entity<UpdatedEntity>()
					.HasRequired(x => x.JoinedEntity1)
					.WithMany();
				modelBuilder.Entity<UpdatedEntity>()
					.HasRequired(x => x.JoinedEntity2)
					.WithMany(y => y.UpdatedEntities);

				modelBuilder.Entity<JoinedEntity1>()
					.HasKey(x => x.JoinedEntity1Id)
					.HasOptional(x => x.IndirectlyJoinedEntity)
					.WithRequired();

				modelBuilder.ComplexType<RestrictionsComplexType>();

				modelBuilder.Entity<IndirectlyJoinedEntity>()
					.HasKey(x => x.IndirectlyJoinedEntityId);

				base.OnModelCreating(modelBuilder);
            }
		}

	    // for BatchUpdateDelete_TPH
        public class Contract
	    {
	        public int ContractId { get; set; }
	        public DateTime? StartDate { get; set; }
	        public int Months { get; set; }
	        public decimal Charge { get; set; }
	    }

	    public class MobileContract : Contract
	    {
	        public string MobileNumber { get; set; }
	    }

	    public class TvContract : Contract
	    {
	        public PackageType PackageType { get; set; }
	    }

	    public class BroadbandContract : Contract
	    {
	        public int? DownloadSpeed { get; set; }
	    }




        // For BatchUpdateDelete_ComplexType
        public class ContractComplex
	    {
            [Key]
	        public int ContractId { get; set; }
	        public DateTime? StartDate { get; set; }
	        public int Months { get; set; }
	        public decimal Charge { get; set; }
	        public MobileContractComplex MobileContractComplex { get; set; } = new MobileContractComplex();
	        public TvContractComplex TvContractComplex { get; set; } = new TvContractComplex();
            public BroadbandContractComplex BroadbandContractComplex { get; set; } = new BroadbandContractComplex();

        }

	    [ComplexType]
        public class MobileContractComplex
	    {
	        public string MobileNumber { get; set; } = "";
	    }

	    [ComplexType]
        public class TvContractComplex
        {
	        public PackageType? PackageType { get; set; }
	    }

	    [ComplexType]
        public class BroadbandContractComplex
        {
	        public int? DownloadSpeed { get; set; }
	    }

        public enum PackageType
	    {
	        S,
	        M,
	        L,
	        XL
	    }


		// begin BatchDelete_ChangeSet8127_Issue3109
		public class Customer
		{
			public int CustomerID { get; set; }
			public string Name { get; set; }
			public Boolean IsActive { get; set; }

			public int TotalNumberOfOrders { get; set; }
			public ICollection<Order> Orders { get; set; }
		}

		public class Order
		{
			public int OrderID { get; set; }
			public string Name { get; set; }

			public int CustomerID { get; set; }
			public int CompanyID { get; set; }
			public Company Company { get; set; }
		}

		public class Company
		{
			public int CompanyID { get; set; }
			public string Name { get; set; }
		}

		public class UpdatedEntity
		{
			public Guid UpdatedEntityId { get; set; }

			public Guid JoinedEntity1Id { get; set; }
			public JoinedEntity1 JoinedEntity1 { get; set; }

			public Guid JoinedEntity2Id { get; set; }
			public JoinedEntity2 JoinedEntity2 { get; set; }

			public bool IsActive { get; set; }
			public UpdatedEntityStatus Status { get; set; }
			public int RestrictedNumber { get; set; }
			public bool RestrictedBool { get; set; }
		}

		public enum UpdatedEntityStatus
		{
			Valid,
			Invalid,
			Inactive
		}

		public class JoinedEntity1
		{
			public Guid JoinedEntity1Id { get; set; }
			public RestrictionsComplexType Restrictions { get; set; }
			public JoinedEntity1Status Status { get; set; }
			public IndirectlyJoinedEntity IndirectlyJoinedEntity { get; set; }
		}

		public enum JoinedEntity1Status
		{
			Valid,
			Pending,
			Inactive
		}

		public class RestrictionsComplexType
		{
			public bool Restriction1 { get; private set; }

			public bool Restriction2 { get; private set; }
		}

		public class IndirectlyJoinedEntity
		{
			public Guid IndirectlyJoinedEntityId { get; set; }
		}

		public class JoinedEntity2
		{
			public Guid JoinedEntity2Id { get; set; }
			public JoinedEntity2Status Status { get; set; }
			public ICollection<UpdatedEntity> UpdatedEntities { get; set; }
			public string String { get; set; }
		}

		public enum JoinedEntity2Status
		{
			Valid,
			Invalid,
			Inactive
		}

		// end BatchDelete_ChangeSet8127_Issue3109 

	}
}
