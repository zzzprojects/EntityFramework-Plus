using System;
using System.Collections.Generic;
using System.Text;
using Z.EntityFramework.Extensions;
using Z.Expressions;

namespace Z.EntityFramework.Plus
{
    public static class EntityFrameworkPlusManager
    {
        /// <summary>Gets or sets a value indicating whether the library is in community mode (automatically throw an error for all paid features in EF Extensions and C# Eval Expression).</summary>
        /// <value>True if the library is in community mode (automatically throw an error for all paid features in EF Extensions and C# Eval Expression).</value>
        public static bool IsCommunity
        {
            set
            {
                EntityFrameworkManager.IsCommunity = value;
                EvalManager.IsCommunity = value;
            }
        }
    }
}
