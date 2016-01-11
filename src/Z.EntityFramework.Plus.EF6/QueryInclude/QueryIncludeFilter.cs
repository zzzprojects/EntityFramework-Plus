using System;
using System.Collections.Generic;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for query includes.</summary>
    public class QueryIncludeFilter<T1, T2> : IQueryIncludeFilter
    {
        private readonly List<T2> Excluded = new List<T2>();
        private readonly List<T2> Included = new List<T2>();

        public List<T1> Parents;
        public Func<IEnumerable<T2>, List<T2>> predicate;
        public Func<T1, IEnumerable<T2>> selector;

        public void ApplyFilter(List<object> list2)
        {
            Parents = list2.Cast<T1>().ToList();
            foreach (var item in Parents)
            {
                var list = selector(item);
                var includedItems = predicate(list).ToList();
                Included.AddRange(includedItems);
                Excluded.AddRange(list.Except(includedItems));
            }
        }

        public List<object> GetExcludedList()
        {
            return Excluded.Cast<object>().ToList();
        }

        public List<object> GetIncludedList()
        {
            return Included.Cast<object>().ToList();
        }
    }
}