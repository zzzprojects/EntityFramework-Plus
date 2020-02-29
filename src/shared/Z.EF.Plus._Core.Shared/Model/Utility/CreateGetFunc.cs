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
//        /// <summary>
//        ///     Creates get function.
//        /// </summary>
//        /// <param name="type">The type.</param>
//        /// <param name="propertyName">Name of the property.</param>
//        /// <returns>The new get function.</returns>
//        internal static Func<object, object> CreateGetFunc(Type type, string propertyName)
//        {
//            var parameter = Expression.Parameter(typeof (object));
//            var cast = Expression.Convert(parameter, type);
//            var propertyGetter = Expression.Property(cast, propertyName);
//            var castResult = Expression.Convert(propertyGetter, typeof (object));

//            return Expression.Lambda<Func<object, object>>(castResult, parameter).Compile();
//        }

//        internal static Func<object, object> CreateGetFunc(List<Tuple<Type, PropertyInfo>> list)
//        {
//            var parameter = Expression.Parameter(typeof (object));
//            Expression expression = parameter;

//            foreach (var item in list)
//            {
//                if (expression.Type != item.Item1)
//                {
//                    expression = Expression.Convert(expression, item.Item1);
//                }

//                expression = Expression.Property(expression, item.Item2);
//            }

//            expression = Expression.Convert(expression, typeof (object));
//            return Expression.Lambda<Func<object, object>>(expression, parameter).Compile();
//        }

//        internal static Func<object, bool> CreateGetReferenceValidFunc(List<Tuple<Type, PropertyInfo>> list)
//        {
//            var parameter = Expression.Parameter(typeof(object));
//            Expression memberExpression = parameter;

//            Expression expression = null;
//            for (int i = 0; i < list.Count; i++)
//            {
//                var item = list[i];

//                // The last accessor can be null (The value)
//                if (i == list.Count - 1)
//                {
//                    break;
//                }

//                if (memberExpression.Type != item.Item1)
//                {
//                    memberExpression = Expression.Convert(memberExpression, item.Item1);
//                }

//                memberExpression = Expression.Property(memberExpression, item.Item2);

//                var innerExpression = Expression.NotEqual(Expression.Constant(null), memberExpression);
//                expression = expression != null ? Expression.AndAlso(expression, innerExpression) : innerExpression;
//            }

//            if (expression == null)
//            {
//                throw new Exception(ExceptionMessage.GeneralException);
//            }

//            return Expression.Lambda<Func<object, bool>>(expression, parameter).Compile();
//        }
//    }
//}