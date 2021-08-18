#if EF6
using System;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;

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
            return SetDynamic(@this, typeName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Returns a DbSet instance (IQueryable&lt;T&gt;) for access to entities of the given type in the context and the underlying store.</summary>
		/// <param name="this">The @this to act on.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="stringComparison">The string comparison.</param>
        /// <returns>An IQueryable&lt;object&gt;</returns>
		public static IQueryable<object> SetDynamic(this DbContext @this, string typeName, StringComparison stringComparison)
		{ 
			var entityTypes = @this.GetObjectContext().MetadataWorkspace.GetItemCollection(DataSpace.OSpace)
				.Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
				.OfType<EntityType>().ToList();

			if (entityTypes.Count == 0)
			{
                throw new Exception($"Oops! The workspace doesn't contain any entity types in the method SetDynamic.");
			}

			var entityTypesFiltered = entityTypes.Where(x => x.Name.Equals(typeName, stringComparison)).ToList();

			if (entityTypesFiltered.Count == 0)
			{
                throw new Exception($"Oops! No entity type has been found for the '{typeName}' in the method SetDynamic.");
			}
			if (entityTypesFiltered.Count > 1)
			{
                throw new Exception($"Oops! Multiples entity types Has been found for the '{typeName}' in the method SetDynamic.");
			}
            
			var entityType = entityTypesFiltered.Single();
			var clrTypeProperty = entityType.GetType().GetProperty("ClrType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var clrType = (Type)clrTypeProperty.GetValue(entityType, null);

			return (IQueryable<object>)@this.Set(clrType);
		}
	}
}
#endif