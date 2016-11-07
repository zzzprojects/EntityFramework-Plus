using System;

namespace Z.EntityFramework.Plus
{
    [AttributeUsage(AttributeTargets.All)]
    public class AuditDisplayAttribute : Attribute
    {
        public AuditDisplayAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}