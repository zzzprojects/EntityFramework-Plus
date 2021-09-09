using System;
using System.Collections.Generic;
using System.Text;

namespace Z.EntityFramework.Plus
{
	public static partial class AuditExtensions
	{
        /// <summary>Get the audit display name used in the Audit method (use the AuditManager.DefaultConfiguration).</summary>
        /// <param name="this">The type.</param>
        /// <returns>The audit display name used in the Audit method (use the AuditManager.DefaultConfiguration).</returns>
        public static string GetAuditDisplayName(this Type @this)
        {
            return @this.GetAuditDisplayName(AuditManager.DefaultConfiguration);
        }
        /// <summary>Get the audit display name used in the Audit method.</summary>
        /// <param name="this">The type.</param>
        /// <param name="auditConfiguration">The audit configuration.</param>
        /// <returns>The audit display name used in the Audit method.</returns>
        public static string GetAuditDisplayName(this Type @this, AuditConfiguration auditConfiguration)
        {
            return auditConfiguration.EntityNameFactory != null ?
                          auditConfiguration.EntityNameFactory(@this) :
                          @this.Name;
        }
    }
}
