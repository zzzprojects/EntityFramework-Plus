#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;

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
            // TPT ou TPC ==> j'assume que donnerons le bon type... ou on va chercher la base? ==> pas de préférence.
            var entityType = context.Model.FindEntityType(type);

            if (entityType == null)
            {
                throw new Exception($"Oops! The entity type '{type.Name}' could not be found in the model for the method 'SqlServerSetIdentityInsertOn' or 'SqlServerSetIdentityInsertOff'.");
            }
            
            // logique du GetTableNameWithSchema
#if !EFCORE_3X
            var relational = entityType.Relational();
             var name =  !string.IsNullOrEmpty(relational.Schema)
                ? relational.Schema + "." + relational.TableName
                : relational.TableName;
#else
            var entityZ = entityType.ToZInfo();
            var name = !string.IsNullOrEmpty(entityZ.SchemaNameEF)
            ? entityZ.SchemaNameEF + "." + entityZ.TableNameEF
            : entityZ.TableNameEF;
#endif
            var innerConnection = context.Database.GetDbConnection();

            // OPEN the connection if we own it
            if (innerConnection.State == ConnectionState.Closed)
            {
                if(!OnOff)
                {
                    throw new InvalidOperationException("Oops! The method 'SqlServerSetIdentityInsertOff' can only be called with an open connection.");
                }

                try
                { 
                    context.Database.OpenConnection();
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
                command.CommandText = $"SET IDENTITY_INSERT {name} {onOff};";
                command.ExecuteNonQuery();
            }
        }
    }
}
#endif