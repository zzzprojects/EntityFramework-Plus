// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

namespace Z.Test.EntityFramework.Plus
{
    public class Inheritance_TPC_Dog : Inheritance_TPC_Animal
    {
        public int ColumnDog { get; set; }

        public static Inheritance_TPC_Dog Create()
        {
            return new Inheritance_TPC_Dog();
        }
    }
}