using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Z.EntityFramework.Plus;
#if !INMEMORY && !EFCORE_2X
namespace Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant
{
	// si marche pu voir la logic dans getEnumerator relatif au BufferedDataReader
	[TestClass]
	public class RetrySimpleCasTest
	{
		[TestMethod()]
		public void RetrySimpleCasTest_1()
		{
			using (var context = new ModelAndContext.EntityContext())
			{
				context.EntitySimples.RemoveRange(context.EntitySimples); 
				context.SaveChanges();
			}


			// SEED  
			using (var context = new ModelAndContext.EntityContext())
			{
				for (int i = 0; i < 3; i++)
				{
					context.EntitySimples.Add(new EntitySimple { ColumnInt = i, ColumnString = "test" });
				}

				context.SaveChanges();
			}


			ModelAndContext.IsRetryStrat = true;
			try
			{
				// TEST  
				using (var context = new ModelAndContext.EntityContext())
				{
					
					var test1 = context.EntitySimples.DeferredAny().FutureValue();
					var test2 = context.EntitySimples.DeferredAny().FutureValue();

					var testfinal = test2.ValueAsync().Result;
					Assert.IsTrue(testfinal);
				}
			}
			finally
			{

				ModelAndContext.IsRetryStrat = false;
			} 
		}
	}
}
#endif