#if EFCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Z.EntityFramework.Plus
{
	public static class SetDynamicExtensions
	{
		/// <summary>Returns a DbSet instance (IQueryable&lt;T&gt;) for access to entities of the given type in the context and the underlying store.</summary>
		/// <param name="this">The @this to act on.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>An IQueryable&lt;object&gt;</returns>
		public static IQueryable<object> SetDynamic(this DbContext @this, string typeName)
		{ 
			return  SetDynamic(@this, typeName, StringComparison.OrdinalIgnoreCase); 
		}

		/// <summary>Returns a DbSet instance (IQueryable&lt;T&gt;) for access to entities of the given type in the context and the underlying store.</summary>
		/// <param name="this">The @this to act on.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="stringComparison">The string comparison.</param>
        /// <returns>An IQueryable&lt;object&gt;</returns>
		public static IQueryable<object> SetDynamic(this DbContext @this, string typeName, StringComparison stringComparison)
		{
			var entityTypes = @this.Model.GetEntityTypes().ToList();
			if (entityTypes.Count == 0)
			{
                throw new Exception($"Oops! The workspace doesn't contain any entity types in the method SetDynamic.");
			}

			var entityTypesFiltered = entityTypes.Where(x => x.ClrType.Name.Equals(typeName, stringComparison)).ToList();

			if (entityTypesFiltered.Count == 0)
			{
                throw new Exception($"Oops! No entity type has been found for the '{typeName}' in the method SetDynamic.");
			}
			if (entityTypesFiltered.Count > 1)
			{
                throw new Exception($"Oops! Multiples entity types Has been found for the '{typeName}' in the method SetDynamic.");
			}

			var type = entityTypesFiltered.Single().ClrType;  
			var propertyDbContextDependencies = typeof(DbContext).GetProperty("DbContextDependencies", BindingFlags.Instance | BindingFlags.NonPublic);
			var dbContextDependencies = propertyDbContextDependencies.GetValue(@this);
			return (IQueryable<object>)((IDbSetCache)@this).GetOrAddSet(((IDbContextDependencies)dbContextDependencies).SetSource, type);
        }
	}
}
#endif