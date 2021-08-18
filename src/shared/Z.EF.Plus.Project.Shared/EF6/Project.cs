#if EF6
using System;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
	public static class ProjectExtensions
	{
        /// <summary>Allow to select only a few properties of your entity instead of selecting all properties. The entity is not tracked.</summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// An enumerator
        /// </returns>
		public static IEnumerable<T> Project<T>(this IQueryable<T> query, Expression<Func<T, object>> expression) where T : class
		{
			var anonymousBody = expression.Body as NewExpression;
			if (anonymousBody == null)
			{
				throw new Exception("Oops! Invalid expression, the body is not a NewExpression. Please refer to the documentation to get examples about how to use this feature.");
			}

			var anonymousType = anonymousBody.Type;
			if (!anonymousType.Name.StartsWith("<>f__AnonymousType", StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception("Oops! The NewExpression doesn't use an Anonymous Type. Please refer to the documentation to get examples about how to use this feature.");
			}

			// PARAMETER
			var parameter = Expression.Parameter(anonymousType, expression.Parameters[0].Name);
			

			var type = typeof(T);
			var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
			var newExpression = Expression.New(constructor);

			var anonymousArguments = anonymousBody.Arguments;
			var anonymousProperties = anonymousType.GetProperties();

			var memberAssignments = new List<MemberAssignment>();

			List<string> missingProperties = new List<string>();

			for (int i = 0; i < anonymousProperties.Length; i++)
			{
				var property = type.GetProperty(anonymousProperties[i].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				if (property != null && !(anonymousArguments[i] is NewExpression))
				{
					var argument = anonymousArguments[i];

					var memberInfo = anonymousType.GetProperty(anonymousProperties[i].Name, BindingFlags.Instance | BindingFlags.Public);
					var propertyCall = Expression.Property(parameter, memberInfo);


					var valueBind = Expression.Bind(property, propertyCall);

					memberAssignments.Add(valueBind);
				} 
                else if (property != null && anonymousArguments[i] is NewExpression inBodyExpression)
                {

					var inBodyType = property.PropertyType;
                    var memberBodyInBodyAssignments = new List<MemberAssignment>();

                    var propertyConstructor = inBodyType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
                    var newpPopertyExpression = Expression.New(propertyConstructor);
                    var anonymousinBodyType = inBodyExpression.Type;

                    if (!anonymousinBodyType.Name.StartsWith("<>f__AnonymousType", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("Oops! An anonymous type was expected in the `NewExpression` for `UpdateFromQuery`.");
                    }

                    var anonymousInBodyArguments = inBodyExpression.Arguments;
                    var anonymousInBodyProperties = anonymousinBodyType.GetProperties(); 

					for (int j = 0; j < anonymousInBodyProperties.Length; j++)
                    {
                        var propertyInBody = inBodyType.GetProperty(anonymousInBodyProperties[j].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (propertyInBody != null && !(anonymousInBodyArguments[j] is NewExpression))
                        {
							// x.Parent
							var memberInfoParent = anonymousType.GetProperty(anonymousProperties[i].Name, BindingFlags.Instance | BindingFlags.Public);
							var propertyCallParent = Expression.Property(parameter, memberInfoParent);

							// x.Parent.Property
							var memberInfo = anonymousinBodyType.GetProperty(anonymousInBodyProperties[j].Name, BindingFlags.Instance | BindingFlags.Public);
							var propertyCall = Expression.Property(propertyCallParent, memberInfo);

							var valueBindBodyInBody = Expression.Bind(propertyInBody, propertyCall);
                            memberBodyInBodyAssignments.Add(valueBindBodyInBody);
                        }
                        else
                        {
                            missingProperties.Add(inBodyType.Name + "." + anonymousInBodyProperties[j].Name);
                        }
                    }

                    var memberBodyInBodyInit = Expression.MemberInit(newpPopertyExpression, memberBodyInBodyAssignments);

                    var valueBind = Expression.Bind(property, memberBodyInBodyInit);
                    memberAssignments.Add(valueBind);
                }
                else
				{
					missingProperties.Add(anonymousProperties[i].Name);
				}
			}

			if (missingProperties.Count > 0)
			{
				throw new Exception(String.Format("Oops! The following property '{0}' doesn't exists but has been used when creating the Anonymous Type. Please refer to the documentation to get examples about how to use this feature.",
				  string.Join(", ", missingProperties)));
			}

			var memberInit = Expression.MemberInit(newExpression, memberAssignments);

			var genericFunc = typeof(Func<,>).MakeGenericType(anonymousType, typeof(T));
			var methodLambda = typeof(Expression).GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.Name == "Lambda" && x.IsGenericMethod && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(Expression) &&
			 x.GetParameters()[1].ParameterType == typeof(ParameterExpression[])).Single();


			var finalMethod = methodLambda.MakeGenericMethod(genericFunc);


			//var finalUpdateExpression = Expression.Lambda<Func<object, T>>(memberInit, parameter);
			var finalExpression = finalMethod.Invoke(null, new object[] { memberInit, new ParameterExpression[] { parameter } });

		
			// IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
			var selectMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == "Select" && x.IsGenericMethod && x.GetParameters().Length == 2 && x.GetParameters()[1].ParameterType.IsGenericType && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType &&
			x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>)).Single();

			var selectMethodFinal = selectMethod.MakeGenericMethod(anonymousType, typeof(T));

			var castMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == "Cast" && x.IsGenericMethod && x.GetParameters().Length == 1).Single();

			var castMethodFinal = castMethod.MakeGenericMethod(anonymousType);


			// BD call
			// work without AsNoTracking(), but Entity is create out of EF so Why use tracking here?
			var list = query.AsNoTracking().Select(expression).ToList().AsQueryable(); 

			// cast object to anonymousType
			var queryCast = castMethodFinal.Invoke(null, new object[] { list });

			// Select anonymousType to T
			return ((IEnumerable<T>)selectMethodFinal.Invoke(null, new object[] { queryCast, finalExpression }));
		} 
	}
}
#endif