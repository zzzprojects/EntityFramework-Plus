using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus.Mik_Area
{
    [TestClass]
    public static class TestArea
    {
        [AssemblyInitialize]
        public static void CreateContext(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();

            using (var context = new ModelAndContext.EntityContext())
            {
	            ModelAndContext.My.CreateBD(context);
            }

            using (var context = new TestContext())
            {
                ModelAndContext.My.CreateBD(context);
            }
        }
	}
}
