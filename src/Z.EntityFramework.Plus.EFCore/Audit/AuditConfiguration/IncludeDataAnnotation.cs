using System;
using System.Reflection;

#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        public void IncludeDataAnnotation()
        {
            Include(o =>
            {
#if EF5 || EF6
                var type = ObjectContext.GetObjectType(o.GetType());
                var attributes = type.GetCustomAttributes(typeof(AuditIncludeAttribute), true);
#elif EFCORE
                var type = o.GetType();
                var attributes = type.GetTypeInfo().GetCustomAttributes(typeof (AuditIncludeAttribute), true).ToArray();
#endif

                return attributes.Length != 0;
            });

            ExcludeIncludePropertyPredicates.Add((o, s) =>
            {
                try
                {
#if EF5 || EF6
                    var type = ObjectContext.GetObjectType(o.GetType());
#elif EFCORE
                    var type = o.GetType();
#endif

                    MemberInfo member = type.GetProperty(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                    if (member == null)
                    {
                        member = type.GetField(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    }

                    if (member == null)
                    {
                        return null;
                    }

#if EF5 || EF6
                    var attributes = member.GetCustomAttributes(typeof(AuditIncludeAttribute), true);
#elif EFCORE
                    var attributes = member.GetCustomAttributes(typeof (AuditIncludeAttribute), true).ToArray();
#endif

                    if (attributes.Length > 0)
                    {
                        return true;
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }
    }
}