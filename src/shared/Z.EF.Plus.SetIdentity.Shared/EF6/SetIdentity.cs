#if EF6
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Z.EntityFramework.Extensions.Core.SchemaObjectModel;

namespace Z.EntityFramework.Plus
{
    public static class SetIdentityExtensions
    {
        /// <summary>Set IDENTITY_INSERT ON for the specific entity type. The connection will be opened if closed.</summary>
        /// <typeparam name="T">The entity type.<typeparam>
        /// <param name="this">The DbSet.</param>
        public static void SqlServerSetIdentityInsertOn<T>(this DbSet<T> @this) where T : class
        {
            SqlServerSetIdentity(@this.GetDbContext(), typeof(T), true);
        }

        /// <summary>Set IDENTITY_INSERT ON for the specific entity type. The connection will be opened if closed.</summary>
        /// <typeparam name="T">The entity type.<typeparam>
        /// <param name="this">The context.</param>
        public static void SqlServerSetIdentityInsertOn<T>(this DbContext @this) where T : class
        {
            SqlServerSetIdentity(@this, typeof(T), true);
        }

        /// <summary>Set IDENTITY_INSERT ON for the specific entity type. The connection will be opened if closed.</summary>
        /// <param name="this">The context.</param>
        /// <param name="type">The entity type.</param>
        public static void SqlServerSetIdentityInsertOn(this DbContext @this, Type type)
        {
            SqlServerSetIdentity(@this, type, true);
        }

        /// <summary>Set IDENTITY_INSERT OFF for the specific entity type.</summary>
        /// <typeparam name="T">The entity type.<typeparam>
        /// <param name="this">The DbSet.</param>
        public static void SqlServerSetIdentityInsertOff<T>(this DbSet<T> @this) where T : class
        {
            SqlServerSetIdentity(@this.GetDbContext(), typeof(T), false);
        }

        /// <summary>Set IDENTITY_INSERT OFF for the specific entity type.</summary>
        /// <typeparam name="T">The entity type.<typeparam>
        /// <param name="this">The context.</param>
        public static void SqlServerSetIdentityInsertOff<T>(this DbContext @this) where T : class
        {
            SqlServerSetIdentity(@this, typeof(T), false);
        }

        /// <summary>Set IDENTITY_INSERT OFF for the specific entity type.</summary>
        /// <param name="this">The context.</param>
        /// <param name="type">The entity type.</param>
        public static void SqlServerSetIdentityInsertOff(this DbContext @this, Type type)
        {
            SqlServerSetIdentity(@this, type, false);
        }

        internal static void SqlServerSetIdentity(DbContext context, Type type, bool OnOff)
        {
            var entityTypes = context.GetModel().ConceptualModel.EntityTypes;

            var objectType = ObjectContext.GetObjectType(type);

            var typeName = objectType.Name;
            var entityFound = entityTypes.Find(x => x.Name == typeName);
            string schemaTableName = null;
            if (entityFound != null && entityFound.EntityTypeMapping != null && entityFound.EntityTypeMapping.MappingFragment != null
            && entityFound.EntityTypeMapping.MappingFragment.StoreEntitySet != null && !string.IsNullOrEmpty(entityFound.EntityTypeMapping.MappingFragment.StoreEntitySet.Table))
            { 
                schemaTableName = entityFound.EntityTypeMapping.MappingFragment.StoreEntitySet.GetDestinationTableName(context);
            }
            else
            {
                throw new Exception($"Oops! The entity type '{typeName}' could not be found in the model for the method 'SqlServerSetIdentityInsertOn' or 'SqlServerSetIdentityInsertOff'.");
            } 

            var innerConnection = context.Database.Connection;

            // OPEN the connection if we own it
            if (innerConnection.State == ConnectionState.Closed)
            {
                if(!OnOff)
                {
                    throw new InvalidOperationException("Oops! The method 'SqlServerSetIdentityInsertOff' can only be called with an open connection.");
                }

                try
                {
                    innerConnection.Open();
                }
                catch (Exception e)
                {
                    if (e.Message == "Cannot access a disposed object.\r\nObject name: 'Transaction'.")
                    {
                        throw new InvalidOperationException("The transaction associated with the current connection has completed but has not been disposed.  The transaction must be disposed before the connection can be used to execute SQL statements.", e);
                    }
                    throw;
                }

            } 

            using (var command = innerConnection.CreateCommand())
            {
                var onOff = OnOff ? "ON" : "OFF";
                command.CommandText = $"SET IDENTITY_INSERT {schemaTableName} {onOff};";
                command.ExecuteNonQuery();
            }
        }
    }
}
#endif