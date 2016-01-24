// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE
namespace Z.EntityFramework.Plus
{
    internal class ExceptionMessage
    {
        public static string GeneralException = "Oops! A general error has occurred. Please report the issue including the stack trace to our support team: info@zzzprojects.com";
        public static string QueryIncludeFilter_ArgumentExpression = "Oops! immediate method with expression argument are not supported. Filter using \"Where\" extension method instead! For more information, contact us: info@zzzprojects.com";
        public static string QueryIncludeFilter_CreateQueryElement = "Oops! Select projection are not supported in EF+ Query IncludeFilter! For more information, contact us: info@zzzprojects.com";
    }
}
#endif