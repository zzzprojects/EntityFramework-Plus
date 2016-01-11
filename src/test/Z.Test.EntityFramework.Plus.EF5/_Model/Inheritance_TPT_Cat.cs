// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.ComponentModel.DataAnnotations.Schema;

namespace Z.Test.EntityFramework.Plus
{
    [Table("Inheritance_TPT_Cat")]
    public class Inheritance_TPT_Cat : Inheritance_TPT_Animal
    {
        public int ColumnCat { get; set; }

        public static Inheritance_TPT_Cat Create()
        {
            return new Inheritance_TPT_Cat();
        }
    }
}