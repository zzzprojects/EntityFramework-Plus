using System;
using System.Reflection;
#if EFCORE
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        public void DataAnnotationDisplayName()
        {
            EntityNameFactory = DataAnnotationEntityDisplayName;
            PropertyNameFactory = DataAnnotationPropertyDisplayName;
        }

        internal string DataAnnotationEntityDisplayName(Type o)
        {
#if EF5 || EF6
            var attributes = o.GetCustomAttributes(typeof(AuditDisplayAttribute), true);
#elif EFCORE
            var attributes = o.GetTypeInfo().GetCustomAttributes(typeof (AuditDisplayAttribute), true).ToArray();
#endif

            if (attributes.Length > 0)
            {
                return ((AuditDisplayAttribute) attributes[0]).Name;
            }

            return o.Name;
        }

        internal string DataAnnotationPropertyDisplayName(Type o, string memberName)
        {
            try
            {
                MemberInfo member = o.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                if (member == null)
                {
                    member = o.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                }

                if (member == null)
                {
                    // Oops! member not found
                    return memberName;
                }

#if EF5 || EF6
                var attributes = member.GetCustomAttributes(typeof(AuditDisplayAttribute), true);
#elif EFCORE
                var attributes = member.GetCustomAttributes(typeof (AuditDisplayAttribute), true).ToArray();
#endif

                if (attributes.Length > 0)
                {
                    return ((AuditDisplayAttribute) attributes[0]).Name;
                }

                return member.Name;
            }
            catch (Exception)
            {
                // Could probably happen in case of an indexed property
                return memberName;
            }
        }
    }
}