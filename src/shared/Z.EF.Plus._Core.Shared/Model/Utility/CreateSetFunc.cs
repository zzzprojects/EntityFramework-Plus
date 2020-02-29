//// Copyright (c) 2014 Jonathan Magnan (http://jonathanmagnan.com).
//// All rights reserved (http://zzzproject.com/entity-framework-extensions/).
//// Licensed under MIT License (MIT) (http://zentityframework.codeplex.com/license).

//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel
//{
//    internal partial class Utility
//    {
//        internal static Action<object, object> CreateSetFunc(List<Tuple<Type, PropertyInfo>> list)
//        {
//            var parameter1 = Expression.Parameter(typeof (object));
//            var parameter2 = Expression.Parameter(typeof (object));

//            Expression expression = parameter1;

//            foreach (var item in list)
//            {
//                if (expression.Type != item.Item1)
//                {
//                    expression = Expression.Convert(expression, item.Item1);
//                }

//                expression = Expression.Property(expression, item.Item2);
//            }

//            var parameter2Convert = Expression.Convert(parameter2, expression.Type);
//            expression = Expression.Assign(expression, parameter2Convert);
//            return Expression.Lambda<Action<object, object>>(expression, parameter1, parameter2).Compile();
//        }
//    }
//}