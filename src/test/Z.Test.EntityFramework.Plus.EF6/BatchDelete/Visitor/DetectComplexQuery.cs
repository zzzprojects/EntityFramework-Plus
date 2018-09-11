using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public partial class BatchDelete_Visitor
    {
        [TestMethod]
        public void DetectNavigationPropertyAccess()
        {
            Expression<Func<Entity_Complex, bool>> expression = e => e.Info.ColumnInt > 3;
            var visitor = new BatchDeleteVisitor();
            visitor.Visit(expression);
            Assert.IsFalse(visitor.IsSimpleQuery);
        }

        [TestMethod]
        public void DetectNavigationPropertyAccess2()
        {
            Expression<Func<Entity_Complex, bool>> expression = e => e.ID > 3 && e.Info.ColumnInt > 3;
            var visitor = new BatchDeleteVisitor();
            visitor.Visit(expression);
            Assert.IsFalse(visitor.IsSimpleQuery);
        }

        [TestMethod]
        public void DetectJoinOnLetStatementOperator()
        {
            using (var ctx = new TestContext())
            {
                var z = from e in ctx.Entity_Complexes
                    let k = e.Info.Info
                    select e;

                var expression = z.Expression;
                var visitor = new BatchDeleteVisitor();
                visitor.Visit(expression);
                Assert.IsFalse(visitor.IsSimpleQuery);
            }
        }

        [TestMethod]
        public void DetectJoinOperator()
        {
            using (var ctx = new TestContext())
            {
                var z = from e in ctx.Entity_Complexes
                    join p in ctx.Entity_Basics on e.ID equals p.ID
                    select e;

                var expression = z.Expression;
                var visitor = new BatchDeleteVisitor();
                visitor.Visit(expression);
                Assert.IsFalse(visitor.IsSimpleQuery);
            }
        }
    }
}