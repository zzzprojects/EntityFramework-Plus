// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

namespace Z.EntityFramework.Plus
{
    internal class ExceptionMessage
    {
        public static string GeneralException = "Oops! A general error has occurred. Please report the issue including the stack trace to our support team: info@zzzprojects.com";

#if FULL
        public static string BatchOperations_MaxKeyColumns = "Oops! Batch operation only support table with primary keys containing 5 columns or less. For more information, contact us: info@zzzprojects.com";
#endif
#if FULL || AUDIT
        public static string Audit_DbSet_NotFound = "Oops! The audit DbSet cannot be found. Please refer to the documentation to learn how to add a custom audit DbSet. Please report the issue including the stack trace to our support team: info@zzzprojects.com";
        public static string Audit_Key_Null = "Oops! A key must be specified. Please report the issue including the stack trace to our support team: info@zzzprojects.com";
        public static string Audit_Key_OutOfBound = "Oops! The number of argument for the key specified doesn't match with the number of key members. Please report the issue including the stack trace to our support team: info@zzzprojects.com";
#endif
#if FULL || BATCH_DELETE || BATCH_UPDATE
        public static string BatchOperations_PropertyNotFound = "Oops! Mapping for the property '{0}' cannot be found, the current version don't support yet Complex Type, Enum, TPC, TPH and TPT. For more information, contact us: info@zzzprojects.com";
        public static string BatchOperations_AssemblyNotFound = "Oops! The assembly 'Microsoft.EntityFrameworkCore.SqlServer' could not be found. This feature is only supported for SQL Server for .NET Core. For more information, contact us: info@zzzprojects.com";
#endif
#if FULL || QUERY_CACHE
        public static string QueryCache_FirstTagNullOrEmpty = "Oops! The option 'UseFirstTagAsCacheKey' is enabled but we found no tag, an empty tag string, or a null tag string instead. Make sure a tag is provided, and it's not null or empty. For more information, contact us: info@zzzprojects.com";
        public static string QueryCache_UseTagsNullOrEmpty = "Oops! The option 'UseTagsAsCacheKey' is enabled but we found no tag, an empty tag string, or a null tag string instead. Make sure a tag is provided, and it's not null or empty. For more information, contact us: info@zzzprojects.com";    
#endif
#if FULL || QUERY_INCLUDEFILTER
        public static string QueryIncludeFilter_ArgumentExpression = "Oops! immediate method with expression argument are not supported in EF+ Query IncludeFilter. For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeFilter_CreateQueryElement = "Oops! Select projection are not supported in EF+ Query IncludeFilter. For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeFilter_Include = "Oops! 'Include' method from Entity Framework is not supported, use only IncludeFilter method. For more information, contact us: info@zzzprojects.com";
#endif
#if FULL || QUERY_INCLUDEOPTIMIZED
        public static string QueryIncludeOptimized_NodeReduce = "Oops! A node has not been reduced using AllowIncludeSubPath feature. Please report the LINQ query to our support team: info@zzzprojects.com";
        public static string QueryIncludeOptimized_ByPath_MissingPath = "Oops! the following type '{0}' doesn't contains the specified property '{1}'. For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeOptimized_ArgumentExpression = "Oops! immediate method with expression argument are not supported. For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeOptimized_CreateQueryElement = "Oops! Select projection are not supported in EF+ Query IncludeFilter For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeOptimized_Include = "Oops! 'Include' method from Entity Framework is not supported, use only IncludeOptimized method. For more information, contact us: info@zzzprojects.com";
#endif
    }
}