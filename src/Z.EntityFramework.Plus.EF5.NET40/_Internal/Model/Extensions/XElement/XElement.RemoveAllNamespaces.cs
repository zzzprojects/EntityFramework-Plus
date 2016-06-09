// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Linq;
using System.Xml.Linq;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        internal static XElement RemoveAllNamespaces(this XElement e)
        {
            return new XElement(e.Name.LocalName,
                (from n in e.Nodes()
                    select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
                (e.HasAttributes) ?
                    (from a in e.Attributes()
                        where (!a.IsNamespaceDeclaration)
                              && (a.Name.NamespaceName == "" // Doesn't check primary attribute
                                  || e.Attributes().All(x => x == a || a.Name.LocalName != x.Name.LocalName))
                        select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }
    }
}

#endif
#endif