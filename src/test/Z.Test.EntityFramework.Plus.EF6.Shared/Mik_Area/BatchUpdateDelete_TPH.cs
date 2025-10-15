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
    public class BatchUpdateDelete_TPH
    {

        [TestMethod]
        public void BatchUpdateDelete_TPH_1()
        {
            Action action = () =>
            {
                cleannup();

                // SEED
                using (var context = new ModelAndContext.EntityContext())
                {  
                    //4
                    context.TvContracts.Add(new ModelAndContext.TvContract() {Months = 4});
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });

                    //4
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() {Months = 3, MobileNumber = "4524"});
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });

                    //8
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 52});
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 52});
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 51});
                 

                    context.SaveChanges();
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    context.MobileContracts.Where(x => 1==1).Delete();
                    context.BroadbandContracts.Where(x => x.DownloadSpeed == 52).Update(y => new ModelAndContext.BroadbandContract(){ DownloadSpeed = 59});
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    Assert.AreEqual(context.MobileContracts.ToList().Count, 0);
                    Assert.AreEqual(context.BroadbandContracts.Where(x => x.DownloadSpeed == 59).ToList().Count,3);
                    Assert.AreEqual(context.TvContracts.ToList(),4);
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }


        [TestMethod]
        public void BatchUpdateDelete_TPH_2()
        {
            cleannup();

            // SEED
            using (var context = new ModelAndContext.EntityContext())
            {
                //4
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });

                //4
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });

                //8
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 51 });


                context.SaveChanges();
            }

            using (var context = new ModelAndContext.EntityContext())
            { 
                context.Contracts.Delete(); 
            }

            using (var context = new ModelAndContext.EntityContext())
            {
                Assert.AreEqual(context.Contracts.ToList().Count, 0);
                Assert.AreEqual(context.TvContracts.ToList().Count, 0);
                Assert.AreEqual(context.MobileContracts.ToList().Count, 0);
                Assert.AreEqual(context.BroadbandContracts.ToList().Count, 0); 
            } 
        }

        [TestMethod]
        public void BatchUpdateDelete_TPH_3()
        {
            cleannup();

            var now = DateTime.Now;
            // SEED
            using (var context = new ModelAndContext.EntityContext())
            {
                 //4
                context.TvContracts.Add(new ModelAndContext.TvContract() {Months = 4});
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });

                //4
                context.MobileContracts.Add(new ModelAndContext.MobileContract() {Months = 3, MobileNumber = "4524"});
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });

                //8
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 52});
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 52});
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {Months = 6, DownloadSpeed = 50});
                context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() {DownloadSpeed = 51, StartDate = now }); 

                context.SaveChanges();
            }
            using (var context = new ModelAndContext.EntityContext())
            { 
                context.Contracts.Update(x => new ModelAndContext.Contract { StartDate = (x.Months == 1 ? now : now) });
				 
			}

            using (var context = new ModelAndContext.EntityContext())
            {
                Assert.AreEqual(context.Contracts.Where(x=> x.StartDate == now).ToList().Count,16 ); 
                Assert.AreEqual(context.TvContracts.Where(x => x.StartDate == now).ToList().Count, 4);
                Assert.AreEqual(context.MobileContracts.Where(x => x.StartDate == now).ToList().Count, 4);
                Assert.AreEqual(context.BroadbandContracts.Where(x => x.StartDate == now).ToList().Count, 8);
            }
        }


        [TestMethod]
        public void BatchUpdateDelete_TPH_4()
        {
            Action action = () =>
            {
                cleannup();

                // SEED
                using (var context = new ModelAndContext.EntityContext())
                {
                    //4
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });
                    context.TvContracts.Add(new ModelAndContext.TvContract() { Months = 4 });

                    //4
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });
                    context.MobileContracts.Add(new ModelAndContext.MobileContract() { Months = 3, MobileNumber = "4524" });

                    //8
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 52 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { Months = 6, DownloadSpeed = 50 });
                    context.BroadbandContracts.Add(new ModelAndContext.BroadbandContract() { DownloadSpeed = 51 });


                    context.SaveChanges();
                }

                var now = DateTime.Now;

                using (var context = new ModelAndContext.EntityContext())
                {
                    context.Contracts.Update(x => new ModelAndContext.BroadbandContract { StartDate = now });
                }

                using (var context = new ModelAndContext.EntityContext())
                {
                    Assert.AreEqual(context.Contracts.Where(x => x.StartDate == now).ToList().Count, 8);
                    Assert.AreEqual(context.TvContracts.Where(x => x.StartDate == now).ToList().Count, 0);
                    Assert.AreEqual(context.MobileContracts.Where(x => x.StartDate == now).ToList().Count, 0);
                    Assert.AreEqual(context.BroadbandContracts.Where(x => x.StartDate == now).ToList().Count, 8);
                }
            };

            MyIni.RunWithFailLogical(MyIni.GetSetupCasTest(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name), action);
        }


        public static void cleannup()
        {
            using (var context = new ModelAndContext.EntityContext())
            {
                context.TvContracts.RemoveRange(context.TvContracts);
                context.MobileContracts.RemoveRange(context.MobileContracts);
                context.BroadbandContracts.RemoveRange(context.BroadbandContracts);
                context.SaveChanges();
            }
        } 
    }
}
