// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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