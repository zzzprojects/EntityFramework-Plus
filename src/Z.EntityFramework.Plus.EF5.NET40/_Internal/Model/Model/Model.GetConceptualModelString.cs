// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.Data.Entity;
using System.IO;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        /// <summary>
        ///     A DbContext extension method that gets conceptual model string.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The conceptual model string.</returns>
        internal static string GetConceptualModelString(this DbContext @this)
        {
            var model = string.Concat(@this.GetModelName(), ".csdl");

            try
            {
                using (var stream = @this.GetType().Assembly.GetManifestResourceStream(model))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // The error 'The invoked member is not supported in a dynamic assembly'
                // MAY happen if a dynamic assembly is loaded
            }


            // Try with all loaded assembly
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    using (var stream2 = assembly.GetManifestResourceStream(model))
                    {
                        if (stream2 != null)
                        {
                            using (var reader = new StreamReader(stream2))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // The error 'The invoked member is not supported in a dynamic assembly'
                    // MAY happen if a dynamic assembly is loaded
                }
            }

            throw new Exception(ExceptionMessage.GeneralException);
        }
    }
}

#endif
#endif