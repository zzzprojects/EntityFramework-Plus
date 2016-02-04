// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static void AddOrAppend<TKey, TElement>(this Dictionary<TKey, List<TElement>> dictionary, TKey key, TElement element)
        {
            List<TElement> elements;

            if (!dictionary.TryGetValue(key, out elements))
            {
                elements = new List<TElement>();
                dictionary.Add(key, elements);
            }

            elements.Add(element);
        }
    }
}