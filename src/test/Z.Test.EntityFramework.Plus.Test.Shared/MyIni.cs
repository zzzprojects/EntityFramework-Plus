using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
	public class MyIni
	{
		public static CasTestFailSetupe ShowTestFail = CasTestFailSetupe.ShowOnlyDiff;
		public static Dictionary<string, CasTestFailSetupe> DicSetupFailTest = SetDicSetupFailTest();

		public enum CasTestFailSetupe
		{
			Normal,
			ShowOnlyDiff,
			SkipAll,
		}

		public static CasTestFailSetupe GetSetupCasTest(string name)
		{
			var setup = CasTestFailSetupe.Normal;

			if (DicSetupFailTest.ContainsKey(name))
			{
				setup = DicSetupFailTest[name];
			}

			return setup;
		}

		public static Dictionary<string, CasTestFailSetupe> SetDicSetupFailTest()
		{
			var dicSetupFailTestnew = new Dictionary<string, CasTestFailSetupe>();

			if (ShowTestFail != CasTestFailSetupe.Normal)
			{
#if IsCore3Memory || IsCore5Memory || IsCore5
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchUpdate_Keyword.From", ShowTestFail); 
#endif
#if IsEF6
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Visitor.Skip", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.Mik_Area.BatchUpdateDelete_ComplexType.BatchUpdateDelete_ComplexType_1", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.Mik_Area.BatchUpdateDelete_ComplexType.BatchUpdateDelete_ComplexType_4", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.Mik_Area.BatchUpdateDelete_TPH.BatchUpdateDelete_TPH_1", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.Mik_Area.BatchUpdateDelete_TPH.BatchUpdateDelete_TPH_4", ShowTestFail);
#endif
#if IsCore3Memory || IsCore5Memory
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant.IncludeOptimized.IncludeOptimized_1", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_BatchDelayInterval.Thirty", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Executing.Template", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Executing.WhileDelayTemplate", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Executing.WhileTemplate", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Transaction.Commit", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Transaction.Rollback", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchDelete_Visitor.Skip", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchUpdate_Executing.WhileTemplate", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchUpdate_Transaction.Commit", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.BatchUpdate_Transaction.Rollback", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKey.Deferred_Constant", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKey.Deferred_Variable", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKey.QueryCache_CacheKey", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKey.Immediate_Variable", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKey.Immediate_Constant", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_CacheKeyFactory.NotNull", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_FromCache.FromCache_Queryable_WithDefault", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_FromCache.FromCache_QueryDeferred_WithDefault", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_FromCacheAsync.FromCacheAsync_Queryable_WithDefault", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_FromCacheAsync.FromCacheAsync_QueryDeferred_WithDefault", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_Tag.Tag_Equal", ShowTestFail);   
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_Tag.Tag_Expire", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_Tag.Tag_NotEqual", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseFirstTagAsCacheKey.Many", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseFirstTagAsCacheKey.None", ShowTestFail);  
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseFirstTagAsCacheKey.Single", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseTagsAsCacheKey.Many", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseTagsAsCacheKey.None", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryCache_UseTagsAsCacheKey.Single", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where.Many_Enumerator", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where.Many_Property", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where.Single_Enumerator", ShowTestFail);
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where.Single_Property", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where_Async+<Many_EnumeratorAsync>d__0.MoveNext", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeFilter_Where_Async+<Single_EnumeratorAsync>d__4.MoveNext", ShowTestFail);
#endif
#if IsCore2
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant.QueryFuturCache.QueryFuturCache_1", ShowTestFail);
#endif
#if IsCore6
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Many_Many_Many", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Single_Many_Many_Single", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Single_Many_Many_Many", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Single_Many_Single", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Single_Many_Many", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Many_Single_Single", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Many_Single_Many", ShowTestFail); 
				dicSetupFailTestnew.Add("Z.Test.EntityFramework.Plus.QueryIncludeOptimized_ByPath.Many_Many_Many_Single", ShowTestFail); 
#endif
			}

			return dicSetupFailTestnew;
		}

		public static void RunWithFailLogical(CasTestFailSetupe setup, Action action)
		{
			bool hasErrorUniqueName = false;
			if (setup == CasTestFailSetupe.Normal || setup == CasTestFailSetupe.ShowOnlyDiff)
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					if (setup != MyIni.CasTestFailSetupe.ShowOnlyDiff)
					{
						throw;
					}

					hasErrorUniqueName = true;
				}
			}

			if (!hasErrorUniqueName && setup == MyIni.CasTestFailSetupe.ShowOnlyDiff)
			{
				throw new Exception("Aurait du planté!");
			}
		}

		public static void RunWithFailLogical(CasTestFailSetupe setup, Func<Task> action)
		{
			bool hasErrorUniqueName = false;
			if (setup == CasTestFailSetupe.Normal || setup == CasTestFailSetupe.ShowOnlyDiff)
			{
				try
				{
					action().Wait();
				}
				catch (Exception e)
				{
					if (setup != MyIni.CasTestFailSetupe.ShowOnlyDiff)
					{
						throw;
					}

					hasErrorUniqueName = true;
				}
			}

			if (!hasErrorUniqueName && setup == MyIni.CasTestFailSetupe.ShowOnlyDiff)
			{
				throw new Exception("Aurait du planté!");
			}
		}
	}
}
