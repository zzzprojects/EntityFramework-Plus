using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>Collection of query include filter nulls.</summary>
    public class QueryIncludeFilterNullCollection
    {
        /// <summary>Null collection to empty.</summary>
        /// <param name="item">The item.</param>
        /// <param name="childs">The childs.</param>
        public static void NullCollectionToEmpty(object item, List<BaseQueryIncludeFilterChild> childs)
        {
            var paths = new List<string>();

            // GET path for all child
            // EXAMPLE: Single.Many.Many
            foreach (var child in childs)
            {
                var visitor = new QueryIncludeFilterPathVisitor();
                visitor.RootExpression = child.GetFilter();
                visitor.Visit(child.GetFilter());

                paths.Add(string.Join(".", visitor.Paths));
            }

            paths = paths.Distinct().OrderByDescending(x => x.Length).ToList();

            // OPTIMIZE to verify only unique path
            var pathsToVerify = new List<string>();
            foreach (var path in paths)
            {
                if (!pathsToVerify.Any(x => x.StartsWith(path + ".", StringComparison.InvariantCulture)))
                {
                    pathsToVerify.Add(path);
                }
            }

            // VERIFY unique path
            foreach (var path in pathsToVerify)
            {
                CheckNullRecursive(item, path.Split('.').ToList(), 0);
            }
        }

        /// <summary>Check null recursive.</summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="index">Zero-based index of the.</param>
        public static void CheckNullRecursive(object currentItem, List<string> paths, int index)
        {
            var currentItemEnumerable = currentItem as IEnumerable;
            var path = paths[index];

            if (currentItemEnumerable != null)
            {
                foreach (var item in currentItemEnumerable)
                {
                    CheckNullRecursive(item, paths, index);
                }
            }
            else
            {
                var property = currentItem.GetType().GetProperty(path, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (property != null)
                {
                    var accessor = new PropertyOrFieldAccessor(property);

                    var value = accessor.GetValue(currentItem);

                    if (value == null)
                    {
                        CheckAndSetCollection(accessor, currentItem, property.PropertyType, property.PropertyType);
                        return;
                    }

                    if (index + 1 < paths.Count)
                    {
                        CheckNullRecursive(value, paths, index + 1);
                    }
                }
            }
        }

        private static void CheckAndSetCollection(PropertyOrFieldAccessor accessor, object currentItem, Type propertyType, Type originalType)
        {
            if (propertyType.GetGenericArguments().Length == 1)
            {
                object value;

                var genericTypeDefinition = propertyType.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(ICollection<>))
                {
                    value = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(propertyType.GetGenericArguments()[0]));
                }
                else if (genericTypeDefinition == typeof(IList<>))
                {
                    value = Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyType.GetGenericArguments()[0]));
                }
                else
                {
                    // Create an instance of the original type since we know it implement one of the type supported: List<A>
                    value = Activator.CreateInstance(originalType);
                }

                accessor.SetValue(currentItem, value);
            }
            else
            {
                if (propertyType.BaseType != null && propertyType.BaseType != typeof(object))
                {
                    CheckAndSetCollection(accessor, currentItem, propertyType.BaseType, originalType);
                }
            }
        }
    }
}