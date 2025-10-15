#if !INMEMORY && !EFCORE_2X
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	class My
	{
		//TESTCoreIncludeFilterSimple_TODO_NewVersionPlusTestForCore
		public static string DatabaseName = "TODO_ForMikDoThatOnNormalTestCoreBD";

		// [REPLACE] is in Beta.
		public static string ConnectionString =
			("Server=[REPLACE];Initial Catalog = [BD]; Integrated Security = true; Connection Timeout = 300; Persist Security Info=True;TrustServerCertificate=true").Replace("[REPLACE]", Environment.MachineName).Replace("[BD]", DatabaseName);



		public static void DeleteBD(DbContext context)
		{
			try
			{
				context.Database.EnsureCreated();
				context.Database.EnsureDeleted();
			}
			catch (Exception e)
			{
				using (var commande = new SqlCommand("ALTER DATABASE " + DatabaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE " + DatabaseName + " ;", new SqlConnection(My.ConnectionString)))
				{
					commande.Connection.Open();
					commande.ExecuteNonQuery();
				}
			}
		}
	}
}
#endif