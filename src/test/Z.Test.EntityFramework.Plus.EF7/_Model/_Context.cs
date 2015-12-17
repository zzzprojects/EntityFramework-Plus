// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public class EntityContext : DbContext
    {
        public EntityContext()
        {
            Database.EnsureCreated();
        }

        public EntityContext(bool isEnabled, string fixResharper = null, bool? enableFilter1 = null, bool? enableFilter2 = null, bool? enableFilter3 = null, bool? enableFilter4 = null, bool? excludeClass = null, bool? excludeInterface = null, bool? excludeBaseClass = null, bool? excludeBaseInterface = null, bool? includeClass = null, bool? includeInterface = null, bool? includeBaseClass = null, bool? includeBaseInterface = null)
        {
            Database.EnsureCreated();

            if (enableFilter1 != null)
            {
                this.Filter<FilterEntity>(FilterEntityHelper.Filter.Filter1, entities => entities.Where(x => x.ColumnInt != 1), isEnabled);
                if (!isEnabled && enableFilter1.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter1).Enable();
                }
                else if (isEnabled && !enableFilter1.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter1).Disable();
                }
            }
            if (enableFilter2 != null)
            {
                this.Filter<IFilterEntity>(FilterEntityHelper.Filter.Filter2, entities => entities.Where(x => x.ColumnInt != 2), isEnabled);
                if (!isEnabled && enableFilter2.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter2).Enable();
                }
                else if (isEnabled && !enableFilter2.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter2).Disable();
                }
            }
            if (enableFilter3 != null)
            {
                this.Filter<BaseFilterEntity>(FilterEntityHelper.Filter.Filter3, entities => entities.Where(x => x.ColumnInt != 3), isEnabled);
                if (!isEnabled && enableFilter3.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter3).Enable();
                }
                else if (isEnabled && !enableFilter3.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter3).Disable();
                }
            }
            if (enableFilter4 != null)
            {
                this.Filter<IBaseFilterEntity>(FilterEntityHelper.Filter.Filter4, entities => entities.Where(x => x.ColumnInt != 4), isEnabled);
                if (!isEnabled && enableFilter4.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter4).Enable();
                }
                else if (isEnabled && !enableFilter4.Value)
                {
                    this.Filter(FilterEntityHelper.Filter.Filter4).Disable();
                }
            }

            if (excludeClass != null && excludeClass.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter1).Disable(typeof (FilterEntity));
            }

            if (excludeInterface != null && excludeInterface.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter2).Disable(typeof (IFilterEntity));
            }

            if (excludeBaseClass != null && excludeBaseClass.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter3).Disable(typeof (BaseFilterEntity));
            }

            if (excludeBaseInterface != null && excludeBaseInterface.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter4).Disable(typeof (IBaseFilterEntity));
            }

            if (includeClass != null && includeClass.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter1).Enable(typeof (IFilterEntity));
            }

            if (includeInterface != null && includeInterface.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter2).Enable(typeof (IFilterEntity));
            }

            if (includeBaseClass != null && includeBaseClass.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter3).Enable(typeof (BaseFilterEntity));
            }

            if (includeBaseInterface != null && includeBaseInterface.Value)
            {
                this.Filter(FilterEntityHelper.Filter.Filter4).Enable(typeof (IBaseFilterEntity));
            }
        }

        public DbSet<EntitySimple> EntitySimples { get; set; }

        public DbSet<FilterEntity> FilterEntities { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SqlConnection(ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString));
        }
    }
}