#if EF6
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static int? GetModelCacheKey(this DbContext @this)
        {
            // TBD: Do we want to move this to EF Extensions? We will need to create a list eventually! Let keep it here "temporary" as we don't want to make it public either yet
            var internalContextProperty = @this.GetType().GetProperty("InternalContext", BindingFlags.NonPublic | BindingFlags.Instance);

            if (internalContextProperty != null)
            {
                var internalContext = internalContextProperty.GetValue(@this, null);

                if (internalContext != null)
                {
                    var _cacheKeyFactoryField = internalContext.GetType().GetField("_cacheKeyFactory", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (_cacheKeyFactoryField != null)
                    {
                        try
                        {
                            var functionForCacheKey = (Func<DbContext, IDbModelCacheKey>)_cacheKeyFactoryField.GetValue(internalContext);

                            if (functionForCacheKey != null)
                            {
                                var modelCacheKey = functionForCacheKey(@this);
                                return modelCacheKey.GetHashCode();
                            }
                        }
                        catch
                        {
                            // empty cache, we don't want to throw an error if the function doesn't work for any reason
                        }
                    }
                }
            }

            return null;
        }
    }
}

#endif