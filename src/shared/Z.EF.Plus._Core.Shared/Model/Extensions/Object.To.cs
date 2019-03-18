// Copyright (c) 2014 Jonathan Magnan (http://jonathanmagnan.com).
// All rights reserved (http://zzzproject.com/entity-framework-extensions/).
// Licensed under MIT License (MIT) (http://zentityframework.codeplex.com/license).

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.ComponentModel;

namespace Z.EF.Plus.BatchUpdate.Shared.Extensions
{
	public static partial class DbModelPlusExtentions
	{
		/// <summary>
		///     A System.Object extension method that toes the given this.
		/// </summary>
		/// <typeparam name="T">Generic type parameter.</typeparam>
		/// <param name="this">this.</param>
		/// <returns>A T.</returns>
		internal static T To<T>(this Object @this)
		{
			if (@this != null)
			{
				Type targetType = typeof(T);

				if (@this.GetType() == targetType)
				{
					return (T) @this;
				}

				TypeConverter converter = TypeDescriptor.GetConverter(@this);
				if (converter != null)
				{
					if (converter.CanConvertTo(targetType))
					{
						return (T) converter.ConvertTo(@this, targetType);
					}
				}

				converter = TypeDescriptor.GetConverter(targetType);
				if (converter != null)
				{
					if (converter.CanConvertFrom(@this.GetType()))
					{
						return (T) converter.ConvertFrom(@this);
					}
				}

				if (@this == DBNull.Value)
				{
					return (T) (object) null;
				}
			}

			return (T) @this;
		}

		/// <summary>
		///     A System.Object extension method that toes the given this.
		/// </summary>
		/// <param name="this">this.</param>
		/// <param name="type">The type.</param>
		/// <returns>An object.</returns>
		/// <example>
		///     <code>
		/// using System;
		/// using Microsoft.VisualStudio.TestTools.UnitTesting;
		/// 
		/// 
		/// namespace ExtensionMethods.Examples
		/// {
		/// [TestClass]
		/// public class System_Object_To
		/// {
		/// [TestMethod]
		/// public void To()
		/// {
		/// string nullValue = null;
		/// string value = &quot;1&quot;;
		/// object dbNullValue = DBNull.Value;
		/// 
		/// // Exemples
		/// var result1 = value.To&lt;int&gt;(); // return 1;
		/// var result2 = value.To&lt;int?&gt;(); // return 1;
		/// var result3 = nullValue.To&lt;int?&gt;(); // return null;
		/// var result4 = dbNullValue.To&lt;int?&gt;(); // return null;
		/// 
		/// // Unit Test
		/// Assert.AreEqual(1, result1);
		/// Assert.AreEqual(1, result2.Value);
		/// Assert.IsFalse(result3.HasValue);
		/// Assert.IsFalse(result4.HasValue);
		/// }
		/// }
		/// }
		/// </code>
		/// </example>
		/// <example>
		///     <code>
		/// using System;
		/// using Microsoft.VisualStudio.TestTools.UnitTesting;
		/// 
		/// 
		/// namespace ExtensionMethods.Examples
		/// {
		/// [TestClass]
		/// public class System_Object_To
		/// {
		/// [TestMethod]
		/// public void To()
		/// {
		/// string nullValue = null;
		/// string value = &quot;1&quot;;
		/// object dbNullValue = DBNull.Value;
		/// 
		/// // Exemples
		/// var result1 = value.To&lt;int&gt;(); // return 1;
		/// var result2 = value.To&lt;int?&gt;(); // return 1;
		/// var result3 = nullValue.To&lt;int?&gt;(); // return null;
		/// var result4 = dbNullValue.To&lt;int?&gt;(); // return null;
		/// 
		/// // Unit Test
		/// Assert.AreEqual(1, result1);
		/// Assert.AreEqual(1, result2.Value);
		/// Assert.IsFalse(result3.HasValue);
		/// Assert.IsFalse(result4.HasValue);
		/// }
		/// }
		/// }
		/// </code>
		/// </example>
		internal static object To(this Object @this, Type type)
		{
			if (@this != null)
			{
				Type targetType = type;

				if (@this.GetType() == targetType)
				{
					return @this;
				}

				TypeConverter converter = TypeDescriptor.GetConverter(@this);
				if (converter != null)
				{
					if (converter.CanConvertTo(targetType))
					{
						return converter.ConvertTo(@this, targetType);
					}
				}

				converter = TypeDescriptor.GetConverter(targetType);
				if (converter != null)
				{
					if (converter.CanConvertFrom(@this.GetType()))
					{
						return converter.ConvertFrom(@this);
					}
				}

				if (@this == DBNull.Value)
				{
					return null;
				}
			}

			return @this;
		}
	}
}
#endif
#endif