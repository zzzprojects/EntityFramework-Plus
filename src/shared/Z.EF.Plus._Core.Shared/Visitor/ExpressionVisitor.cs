// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_UPDATE
using System;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    internal static class ExpressionVisitorExtensions
    {
        public static Expression Visit<TExpression>(this Expression expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }

        public static TReturn Visit<TExpression, TReturn>(this TReturn expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
            where TReturn : Expression
        {
            return (TReturn) ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }

        public static Expression<TDelegate> Visit<TExpression, TDelegate>(this Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }
    }

    internal class ExpressionVisitor<TExpression> : ExpressionVisitor where TExpression : Expression
    {
        private readonly Func<TExpression, Expression> _visitor;

        public ExpressionVisitor(Func<TExpression, Expression> visitor)
        {
            _visitor = visitor;
        }

        public override Expression Visit(Expression expression)
        {
            if (expression is TExpression && _visitor != null)
                expression = _visitor(expression as TExpression);

            return base.Visit(expression);
        }

        public static Expression Visit(Expression expression, Func<TExpression, Expression> visitor)
        {
            return new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }

        public static Expression<TDelegate> Visit<TDelegate>(Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
        {
            return (Expression<TDelegate>) new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }
    }
}
#endif