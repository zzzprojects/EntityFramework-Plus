using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;
using Z.Test.EntityFramework.Plus.EFCore.Shared.MikaelAreaIndependant;
using Z.Test.EntityFramework.Plus.Mik_Area;
#if EFCORE_3X
namespace Z.Test.EntityFramework.Plus.EF6.Shared.Mik_Area
{
    [TestClass]
    public class WhereBulkContain
    {
        public static void Clean()
        {
            using (var context = new ModelAndContext.EntityContext())
            {
                context.EntitySimples.RemoveRange(context.EntitySimples); 
                context.SaveChanges();
            }
        }

        //[TestMethod]
        //public void Test_1()
        //{
        //    Clean();

        //    var entity = new EntitySimple();
          
        //        using (var context = new ModelAndContext.EntityContext())
        //        {
        //            entity.ColumnInt = 125; 

        //            context.EntitySimples.Add(entity);
                 
        //            context.SaveChanges();


        //        }

        //        using (var context = new ModelAndContext.EntityContext())
        //        {
        //        var dbco = context.Database.GetDbConnection();
        //        dbco.Open();
        //            var entityEFplus = context.EntitySimples.WhereBulkContains(new List<int>() { 1 }, x => x.ColumnInt).Future();

        //        var entityEFplu2s = context.EntitySimples.WhereBulkContains(new List<int>() { 125 }, x => x.ColumnInt).Future();

        //        var check = entityEFplus.ToList();
        //        var check2 = entityEFplu2s.ToList();

        //        Assert.AreEqual(0, check.Count);
        //        Assert.AreEqual(1, check2.Count);

        //    }
        //} 
    }
}
#endif
