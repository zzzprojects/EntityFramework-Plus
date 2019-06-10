#if EFCORE && !EFCORE_3X
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Internal;

namespace Z.EntityFramework.Plus
{
    public class LazyHelper
    {
        public static object NewLazy<T>(Func<T> valueFactory)
        {
            if (!EFCoreHelper.IsVersion3xPreview5)
            {
                return LazyRefHelper.NewLazy<T>(valueFactory);
            }
            else
            {
                return new Lazy<T>(valueFactory);
            }
        }
    }

    public class LazyRefHelper
    {
        public static object NewLazy<T>(Func<T> valueFactory)
        {
            return new LazyRef<T>(valueFactory);
        }
    }
}
#endif