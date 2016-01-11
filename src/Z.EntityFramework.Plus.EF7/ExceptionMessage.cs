// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

namespace Z.EntityFramework.Plus
{
    public class ExceptionMessage
    {
        public static string GeneralException = "Oops! Something go wrong. Please report this issue with the stack trace to our support team: info@zzzprojects.com";
        public static string QueryIncludeQuery_ArgumentExpression = "Oops! immediate method with expression argument are not supported. For more info, contact us: info@zzzprojects.com";
        public static string QueryIncludeQuery_ToManyInclude = "Oops! the library is limited to 9 include. Contact us if you need we lift this limit for you: info@zzzprojects.com";
    }
}