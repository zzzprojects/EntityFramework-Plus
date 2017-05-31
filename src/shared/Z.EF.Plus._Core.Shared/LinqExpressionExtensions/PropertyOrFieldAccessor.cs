// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_UPDATE || QUERY_INCLUDEFILTER || QUERY_INCLUDEOPTIMIZED

using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal class PropertyOrFieldAccessor
    {
        /// <summary>Constructor.</summary>
        /// <param name="propertyOrFieldPaths">The FieldPaths.</param>
        public PropertyOrFieldAccessor(ReadOnlyCollection<MemberInfo> propertyOrFieldPaths)
        {
            PropertyOrFieldPaths = propertyOrFieldPaths;
            PropertyOrField = propertyOrFieldPaths.LastOrDefault();
        }

        /// <summary>Constructor.</summary>
        /// <param name="property">The property.</param>
        public PropertyOrFieldAccessor(MemberInfo property)
        {
            if (property != null)
            {
                PropertyOrFieldPaths = new ReadOnlyCollection<MemberInfo>(new[] {property});
                PropertyOrField = property;
            }
        }

        /// <summary>Gets or sets the FieldPaths.</summary>
        /// <value>The FieldPaths.</value>
        public ReadOnlyCollection<MemberInfo> PropertyOrFieldPaths { get; internal set; }

        /// <summary>Gets or sets the property.</summary>
        /// <value>The property.</value>
        public MemberInfo PropertyOrField { get; set; }

        /// <summary>
        ///     Gets a value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The value.</returns>
        public object GetValue(object obj)
        {
            var value = obj;
            foreach (var item in PropertyOrFieldPaths)
            {
                if (item is PropertyInfo)
                {
                    value = (item as PropertyInfo).GetValue(value, null);
                }
                else if (item is FieldInfo)
                {
                    value = (item as FieldInfo).GetValue(value);
                }
            }

            return value;
        }

        /// <summary>
        ///     Gets a value.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(object obj)
        {
            var value = obj;
            foreach (var item in PropertyOrFieldPaths)
            {
                if (item is PropertyInfo)
                {
                    value = (item as PropertyInfo).GetValue(value, null);
                }
                else if (item is FieldInfo)
                {
                    value = (item as FieldInfo).GetValue(value);
                }
            }

            return (T) value;
        }

        /// <summary>
        ///     Sets a value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            for (var i = 0; i < PropertyOrFieldPaths.Count; i++)
            {
                var item = PropertyOrFieldPaths[i];
                if (i == PropertyOrFieldPaths.Count - 1)
                {
                    if (item is PropertyInfo)
                    {
                        (item as PropertyInfo).SetValue(obj, value, null);
                    }
                    else if (item is FieldInfo)
                    {
                        (item as FieldInfo).SetValue(obj, value);
                    }
                }
                else
                {
                    if (item is PropertyInfo)
                    {
                        obj = (item as PropertyInfo).GetValue(obj, null);
                    }
                    else if (item is FieldInfo)
                    {
                        obj = (item as FieldInfo).GetValue(obj);
                    }
                }
            }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Join(".", PropertyOrFieldPaths.Select(x => x.Name).ToArray());
        }
    }
}
#endif