using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchDelete_Visitor
    {
        [TestMethod]
        public void DetectSimpleQuery()
        {
            Expression<Func<Entity_Complex, bool>> expression = e => e.ID > 3;
            var visitor = new BatchDeleteVisitor();
            visitor.Visit(expression);
            Assert.IsTrue(visitor.IsSimpleQuery);
        }
    }
}