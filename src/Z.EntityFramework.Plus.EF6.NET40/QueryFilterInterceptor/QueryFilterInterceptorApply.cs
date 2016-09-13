using System;
using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    public class QueryFilterInterceptorApply
    {
        /// <summary>List of apply filters.</summary>
        public List<Func<BaseQueryFilterInterceptor, bool?>> ApplyFilterList = new List<Func<BaseQueryFilterInterceptor, bool?>>();

        /// <summary>The global filters.</summary>
        public QueryFilterContextInterceptor GlobalFilters;

        /// <summary>The instance filters.</summary>
        public QueryFilterContextInterceptor InstanceFilters;

        /// <summary>Query if 'filter' is enabled.</summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>true if enabled, false if not.</returns>
        public bool? IsEnabled(BaseQueryFilterInterceptor filter)
        {
            bool? isEnabled = null;

            foreach (var applyFilter in ApplyFilterList)
            {
                var shouldEnable = applyFilter(filter);

                if (shouldEnable.HasValue)
                {
                    isEnabled = shouldEnable.Value;
                }
            }

            return isEnabled;
        }
    }
}