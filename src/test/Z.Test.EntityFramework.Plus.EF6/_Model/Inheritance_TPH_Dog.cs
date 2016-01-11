// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

namespace Z.Test.EntityFramework.Plus
{
    public class Inheritance_TPH_Dog : Inheritance_TPH_Animal
    {
        public int ColumnDog { get; set; }

        public static Inheritance_TPH_Dog Create()
        {
            return new Inheritance_TPH_Dog();
        }
    }
}