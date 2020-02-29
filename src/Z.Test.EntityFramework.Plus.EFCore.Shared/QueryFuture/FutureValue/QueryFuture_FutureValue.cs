using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class QueryFuture_FutureValue
    {
        [TestMethod]
        public void Try_Cast_Null_FutureValue_To_Certain_Type_Should_Return_Null()
        {
            QueryFutureValue<string> futureValue = null;
            string tmp = futureValue;

            Assert.IsNull(tmp);
        }
    }
}