// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        /// <summary>
        ///     A DbContext extension method that gets model name.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The model name.</returns>
        internal static List<string> GetModelNames(this DbContext @this)
        {
            var list = new List<string>();

            var workspaces = @this.GetObjectContext().MetadataWorkspace.GetItems(DataSpace.CSSpace);

            if (workspaces.Count > 0)
            {
                foreach (var workspace in workspaces)
                {
                    var sourceLocationProperty = workspace.GetType().GetProperty("SourceLocation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

                    if (sourceLocationProperty != null)
                    {
                        var sourceLocation = (string)sourceLocationProperty.GetValue(workspace, null);

                        var lastIndex = sourceLocation.LastIndexOf("/");
                        sourceLocation = sourceLocation.Substring(lastIndex + 1).Replace(".msl", "");
                        list.Add(sourceLocation);
                    }
                }
            }

            return list;
        }
    }
}

#endif
#endif