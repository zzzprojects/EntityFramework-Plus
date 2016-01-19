// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE
using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>
        ///     A Dictionary&lt;TKey,List&lt;TElement&gt;&gt; extension method that adds or appends an
        ///     element to a list associated to the key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TElement">Type of the element.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="element">The element.</param>
        internal static void AddOrAppend<TKey, TElement>(this Dictionary<TKey, List<TElement>> dictionary, TKey key, TElement element)
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
#endif