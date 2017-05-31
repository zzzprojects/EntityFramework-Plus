#if EF6
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>Collection of query include optimized nulls.</summary>
    public class QueryIncludeFilterLazyLoading
    {
        /// <summary>Null collection to empty.</summary>
        /// <param name="item">The item.</param>
        /// <param name="childs">The childs.</param>
        public static void SetLazyLoaded(object item, List<BaseQueryIncludeFilterChild> childs)
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
                SetLazyLoaded(item, path.Split('.').ToList(), 0);
            }
        }

        /// <summary>Check null recursive.</summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="index">Zero-based index of the.</param>
        public static void SetLazyLoaded(object currentItem, List<string> paths, int index)
        {
            var currentItemEnumerable = currentItem as IEnumerable;
 
            var path = paths[index];

            if (currentItemEnumerable != null)
            {
                foreach (var item in currentItemEnumerable)
                {
                    SetLazyLoaded(item, paths, index);
                }
            }
            else
            {
                if (!currentItem.GetType().FullName.StartsWith("System.Data.Entity.DynamicProxies.", StringComparison.InvariantCulture)) return;

                var entityWrapperField = currentItem.GetType().GetField("_entityWrapper", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var entityWrapper = entityWrapperField.GetValue(currentItem);

                var relationshipManagerProperty = entityWrapper.GetType().GetProperty("RelationshipManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var relationshipManager = (System.Data.Entity.Core.Objects.DataClasses.RelationshipManager)relationshipManagerProperty.GetValue(entityWrapper, null);

                foreach (var related in relationshipManager.GetAllRelatedEnds())
                {
                    var navigationPropertyProperty = related.GetType().GetProperty("NavigationProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    var navigationProperty = (System.Data.Entity.Core.Metadata.Edm.NavigationProperty)navigationPropertyProperty.GetValue(related, null);

                    if (navigationProperty != null && navigationProperty.Name == path)
                    {
                        related.IsLoaded = true;

                        var property = currentItem.GetType().GetProperty(path, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        if (property != null)
                        {
                            var accessor = new PropertyOrFieldAccessor(property);

                            var value = accessor.GetValue(currentItem);

                            if (value == null)
                            {
                                return;
                            }

                            if (index + 1 < paths.Count)
                            {
                                SetLazyLoaded(value, paths, index + 1);
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif