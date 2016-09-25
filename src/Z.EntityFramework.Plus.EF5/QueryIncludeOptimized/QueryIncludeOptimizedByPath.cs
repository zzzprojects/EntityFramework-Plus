using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A query include optimized by path.</summary>
    public static class QueryIncludeOptimizedByPath
    {
        /// <summary>Include optimized by path.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="navigationPath">Full pathname of the navigation file.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> IncludeOptimizedByPath<T>(IQueryable<T> query, string navigationPath)
        {
            var elementType = typeof (T);
            var paths = navigationPath.Split('.');

            // CREATE expression x => x.Right
            var expression = CreateLambdaExpression(elementType, paths, 0);

            var method = typeof (QueryIncludeOptimizedExtensions)
                .GetMethod("IncludeOptimizedSingle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(typeof (T), expression.Type.GetGenericArguments()[1]);

            query = (IQueryable<T>) method.Invoke(null, new object[] {query, expression});

            return query;
        }

        /// <summary>Creates lambda expression.</summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>The new lambda expression.</returns>
        public static Expression CreateLambdaExpression(Type parameterType, string[] paths, int currentIndex)
        {
            // CREATE expression [x => x.Right]

            // ADD parameter [x =>]
            var parameter = Expression.Parameter(parameterType);
            Expression expression = parameter;

            // ADD property [x.Right]
            expression = AppendPropertyPath(expression, paths, currentIndex);

            // GET function generic type
            var funcGenericType = typeof (Func<,>).MakeGenericType(parameterType, expression.Type);

            // GET lambda method
            var lambdaMethod = typeof (Expression).GetMethods()
                .Single(x => x.Name == "Lambda"
                             && x.IsGenericMethod
                             && x.GetParameters().Length == 2
                             && !x.GetParameters()[1].ParameterType.IsArray)
                .MakeGenericMethod(funcGenericType);

            // CREATE lambda expression
            expression = (Expression) lambdaMethod.Invoke(null, new object[] {expression, new List<ParameterExpression> {parameter}});

            return expression;
        }

        /// <summary>Appends a path.</summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>An Expression.</returns>
        public static Expression AppendPath(Expression expression, string[] paths, int currentIndex)
        {
            expression = expression.Type.GetGenericArguments().Length == 0 ?
                AppendPropertyPath(expression, paths, currentIndex) :
                AppendSelectPath(expression, paths, currentIndex);

            return expression;
        }

        /// <summary>Appends a property path.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>An Expression.</returns>
        public static Expression AppendPropertyPath(Expression expression, string[] paths, int currentIndex)
        {
            // APPEND [x.PropertyName]
            var elementType = expression.Type;
            var property = elementType.GetProperty(paths[currentIndex], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // ENSURE property exists
            if (property == null)
            {
                throw new Exception(string.Format(ExceptionMessage.QueryIncludeOptimized_ByPath_MissingPath, elementType.FullName, paths[currentIndex]));
            }

            expression = Expression.Property(expression, property);

            // APPEND path childs
            currentIndex++;
            if (currentIndex < paths.Length)
            {
                expression = AppendPath(expression, paths, currentIndex);
            }

            return expression;
        }

        /// <summary>Appends a select path.</summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>An Expression.</returns>
        public static Expression AppendSelectPath(Expression expression, string[] paths, int currentIndex)
        {
            // APPEND x => x.Rights[.Select(y => y.Right)]
            var elementType = expression.Type.GetGenericArguments()[0];

            // CREATE lambda expression [y => y.Right]
            var lambdaExpression = CreateLambdaExpression(elementType, paths, currentIndex);

            // APPEND Method [.Select(y => y.Right)]
            var selectMethod = typeof (Enumerable).GetMethods()
                .Single(x => x.Name == "Select"
                             && x.GetParameters().Length == 2
                             && x.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2)
                .MakeGenericMethod(elementType, lambdaExpression.Type.GetGenericArguments()[1]);
            expression = Expression.Call(null, selectMethod, expression, lambdaExpression);

            return expression;
        }
    }
}