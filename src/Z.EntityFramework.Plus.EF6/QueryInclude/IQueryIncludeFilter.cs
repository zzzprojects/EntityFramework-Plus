using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for query includes.</summary>
    public interface IQueryIncludeFilter
    {
        void ApplyFilter(List<object> list);

        List<object> GetIncludedList();
        List<object> GetExcludedList();

    }
}