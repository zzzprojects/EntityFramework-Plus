using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
    [TestClass]
    public class BatchUpdateDelete_ComplexType
    {

        [TestMethod]
        public void BatchUpdateDelete_ComplexType_1()
        {
            Action action = () =>
            {
                cleannup();

                // SEED
                using (var context = new ModelAndContext.EntityContext())
                {  
                    //4
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4});
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });

                    //4
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() {Months = 3, MobileContractComplex= new ModelAndContext.MobileContractComplex(){ MobileNumber = "4524"}});
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });

                    //8
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() {  DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() {  BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 52 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 52 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 1 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 52 }, TvContractComplex =  new ModelAndContext.TvContractComplex() {PackageType =  ModelAndContext.PackageType.L}});
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } , TvContractComplex =  new ModelAndContext.TvContractComplex() {PackageType =  ModelAndContext.PackageType.L}});


                    context.SaveChanges();
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    // can't be null, but maybe I can do this where for get nothing
                    context.ContractComplexs.Where(x => x.MobileContractComplex != null).Delete();

                    context.ContractComplexs.Where(x => x.MobileContractComplex.MobileNumber == "4524").Delete();
                    context.ContractComplexs.Where(x => x.BroadbandContractComplex.DownloadSpeed == 52).Update(y => new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 59}});
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    Assert.AreEqual(context.ContractComplexs.ToList().Count, 12);
                    Assert.AreEqual(context.ContractComplexs.Where(x => x.BroadbandContractComplex.DownloadSpeed == 59).ToList().Count,3); 
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }


        [TestMethod]
        public void BatchUpdateDelete_ComplexType_2()
        {
            cleannup();

            // SEED
            using (var context = new ModelAndContext.EntityContext())
            {
                //4
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });

                //4
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });

                //8
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });



                context.SaveChanges();
            }

            using (var context = new ModelAndContext.EntityContext())
            { 
                context.ContractComplexs.Delete(); 
            }

            using (var context = new ModelAndContext.EntityContext())
            {
                Assert.AreEqual(context.ContractComplexs.ToList().Count, 0); 
            } 
        }

        [TestMethod]
        public void BatchUpdateDelete_ComplexType_3()
        {
            cleannup();
            
            // SEED
            using (var context = new ModelAndContext.EntityContext())
            {
                //4
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });

                //4
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });

                //8
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });
                context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });


                context.SaveChanges();
            }
            using (var context = new ModelAndContext.EntityContext())
            { 
                context.ContractComplexs.Where(x => x.MobileContractComplex.MobileNumber != "4524").Update(x => new ModelAndContext.ContractComplex() { MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "100" } }); 
            }

            using (var context = new ModelAndContext.EntityContext())
            {
                Assert.AreEqual(context.ContractComplexs.Where(x => x.MobileContractComplex.MobileNumber == "100").ToList().Count,12 );  
            }
        }


        [TestMethod]
        public void BatchUpdateDelete_ComplexType_4()
        {
            Action action = () =>
            {
                cleannup();

                // SEED
                using (var context = new ModelAndContext.EntityContext())
                {
                    //4
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 4 });

                    //4
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 3, MobileContractComplex = new ModelAndContext.MobileContractComplex() { MobileNumber = "4524" } });

                    //8
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { Months = 6, BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });
                    context.ContractComplexs.Add(new ModelAndContext.ContractComplex() { BroadbandContractComplex = new ModelAndContext.BroadbandContractComplex() { DownloadSpeed = 50 }, TvContractComplex = new ModelAndContext.TvContractComplex() { PackageType = ModelAndContext.PackageType.L } });



                    context.SaveChanges();
                }

                var now = DateTime.Now;

                using (var context = new ModelAndContext.EntityContext())
                {
                    context.ContractComplexs.Update(x => new ModelAndContext.ContractComplex(){StartDate = (x.Months ==1 ?  now : now)   });
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    Assert.AreEqual(context.ContractComplexs.Where(x => x.StartDate == now).ToList().Count, 16); 
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }


        public static void cleannup()
        {
            using (var context = new ModelAndContext.EntityContext())
            {
                context.ContractComplexs.RemoveRange(context.ContractComplexs); 
                context.SaveChanges();
            }
        } 
    }
}
