using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE || QUERY_FILTER || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF5 || EF6

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>A Type extension method that gets a field.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="type">The type to act on.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <param name="recursif">True to recursif.</param>
        /// <returns>The field.</returns>
        public static FieldInfo GetField(this Type type, string name, BindingFlags bindingAttr, bool recursif)
        {
            FieldInfo fieldinfo;
            if (!recursif)
            {
                fieldinfo = type.GetField(name, bindingAttr);
            }
            else
            {
                fieldinfo = type.GetField(name, bindingAttr);

                if(fieldinfo == null && type != typeof(object))
                {
                    fieldinfo = type.BaseType.GetField(name, bindingAttr, true);
                }               
            }

            return fieldinfo;
        }
    }
}

#endif
#endif
