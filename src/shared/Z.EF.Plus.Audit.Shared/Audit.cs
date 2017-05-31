// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>An audit.</summary>
    public partial class Audit
    {
        /// <summary>The lazy configuration.</summary>
        private readonly Lazy<AuditConfiguration> _configuration;

        /// <summary>Default constructor.</summary>
        public Audit()
        {
            _configuration = new Lazy<AuditConfiguration>(() => AuditManager.DefaultConfiguration.Clone());
            Entries = new List<AuditEntry>();

            try
            {
#if !NETSTANDARD1_3
                CreatedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name;
#endif

                if (string.IsNullOrEmpty(CreatedBy))
                {
                    CreatedBy = "System";
                }
            }
            catch (Exception)
            {
                // Oops! it's k, this is the responsability of the user to set the default CreatedBy field
                CreatedBy = "System";
            }
        }

        /// <summary>Gets or sets the entries.</summary>
        /// <value>The entries.</value>
        public List<AuditEntry> Entries { get; set; }

        /// <summary>Gets or sets the  created by username.</summary>
        /// <value>The created by username.</value>
        public string CreatedBy { get; set; }

        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public AuditConfiguration Configuration
        {
            get { return _configuration.Value; }
        }

        /// <summary>Gets the current or default configuration.</summary>
        /// <value>The current or default configuration.</value>
        internal AuditConfiguration CurrentOrDefaultConfiguration
        {
            get { return _configuration.IsValueCreated ? _configuration.Value : AuditManager.DefaultConfiguration; }
        }

        /// <summary>Updates audit entries after the save changes has been executed.</summary>
        public void PostSaveChanges()
        {
            PostSaveChanges(this);
        }

        /// <summary>Adds audit entries before the save changes has been executed.</summary>
        /// <param name="context">The context.</param>
        public void PreSaveChanges(DbContext context)
        {
            PreSaveChanges(this, context);
        }
    }
}