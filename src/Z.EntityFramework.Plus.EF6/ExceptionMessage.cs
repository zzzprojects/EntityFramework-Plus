// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

namespace Z.EntityFramework.Plus
{
    internal class ExceptionMessage
    {
        public static string GeneralException = "Oops! A general error has occurred. Please report the issue including the stack trace to our support team: info@zzzprojects.com";

#if !STANDALONE
        public static string QueryIncludeQuery_ArgumentExpression = "Oops! immediate method with expression argument are not supported. Filter using \"Where\" extension method instead! For more info, contact us: info@zzzprojects.com";
        public static string QueryIncludeQuery_ToManyInclude = "Oops! the library is limited to 9 include. Contact us if you need we lift this limit for you: info@zzzprojects.com";
#endif
    }
}