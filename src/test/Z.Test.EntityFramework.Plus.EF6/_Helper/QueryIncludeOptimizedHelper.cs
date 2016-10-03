using System.Collections.Generic;
using Z.EntityFramework.Plus;

namespace Z.Test.EntityFramework.Plus
{
    public static class QueryIncludeOptimizedHelper
    {
        public static void InsertOneToOneAndMany(string resharperFix = null, bool? single1 = null, bool? single2 = null, bool? single3 = null, bool? single4 = null, bool? many1 = null, bool? many2 = null, bool? many3 = null, bool? many4 = null)
        {
            // CLEAN UP
            using (var ctx2 = new TestContext())
            {
                ctx2.Association_OneToSingleAndMany_RightRightRightRights.RemoveRange(ctx2.Association_OneToSingleAndMany_RightRightRightRights);
                ctx2.Association_OneToSingleAndMany_RightRightRights.RemoveRange(ctx2.Association_OneToSingleAndMany_RightRightRights);
                ctx2.Association_OneToSingleAndMany_RightRights.RemoveRange(ctx2.Association_OneToSingleAndMany_RightRights);
                ctx2.Association_OneToSingleAndMany_Rights.RemoveRange(ctx2.Association_OneToSingleAndMany_Rights);
                ctx2.Association_OneToSingleAndMany_Lefts.RemoveRange(ctx2.Association_OneToSingleAndMany_Lefts);

                ctx2.SaveChanges();
            }

            var ctx = new TestContext();

            var left = ctx.Association_OneToSingleAndMany_Lefts.Add(new Association_OneToSingleAndMany_Left());
            ctx.SaveChanges();

            List<Association_OneToSingleAndMany_Right> rights1 = new List<Association_OneToSingleAndMany_Right>();
            List<Association_OneToSingleAndMany_RightRight> rights2 = new List<Association_OneToSingleAndMany_RightRight>();
            List<Association_OneToSingleAndMany_RightRightRight> rights3 = new List<Association_OneToSingleAndMany_RightRightRight>();
            List<Association_OneToSingleAndMany_RightRightRightRight> rights4 = new List<Association_OneToSingleAndMany_RightRightRightRight>();

            // Level 1
            {
                if (single1.HasValue)
                {
                    if (!single1.Value) return;

                    left.Single_Right = new Association_OneToSingleAndMany_Right();
                    rights1.Add(left.Single_Right);
                }
                else if (many1.HasValue)
                {
                    if (!many1.Value) return;

                    left.Many_Right = new List<Association_OneToSingleAndMany_Right>();
                    left.Many_Right.Add(new Association_OneToSingleAndMany_Right());
                    rights1.Add(left.Many_Right[0]);
                }

                ctx.SaveChanges();
            }

            // Level 2
            {
                if (single2.HasValue)
                {
                    if (!single2.Value) return;

                    foreach (var item in rights1)
                    {
                        item.Single_RightRight = new Association_OneToSingleAndMany_RightRight();
                        rights2.Add(item.Single_RightRight);
                    }


                }
                else if (many2.HasValue)
                {
                    if (!many2.Value) return;

                    foreach (var item in rights1)
                    {
                        item.Many_RightRight = new List<Association_OneToSingleAndMany_RightRight>();
                        item.Many_RightRight.Add(new Association_OneToSingleAndMany_RightRight());
                        rights2.Add(item.Many_RightRight[0]);
                    }
                }

                ctx.SaveChanges();
            }

            // Level 3
            {
                if (single3.HasValue)
                {
                    if (!single3.Value) return;

                    foreach (var item in rights2)
                    {
                        item.Single_RightRightRight = new Association_OneToSingleAndMany_RightRightRight();
                        rights3.Add(item.Single_RightRightRight);
                    }


                }
                else if (many3.HasValue)
                {
                    if (!many3.Value) return;

                    foreach (var item in rights2)
                    {
                        item.Many_RightRightRight = new List<Association_OneToSingleAndMany_RightRightRight>();
                        item.Many_RightRightRight.Add(new Association_OneToSingleAndMany_RightRightRight());
                        rights3.Add(item.Many_RightRightRight[0]);
                    }
                }

                ctx.SaveChanges();
            }

            // Level 4
            {
                if (single4.HasValue)
                {
                    if (!single4.Value) return;

                    foreach (var item in rights3)
                    {
                        item.Single_RightRightRightRight = new Association_OneToSingleAndMany_RightRightRightRight();
                        rights4.Add(item.Single_RightRightRightRight);
                    }


                }
                else if (many4.HasValue)
                {
                    if (!many4.Value) return;

                    foreach (var item in rights3)
                    {
                        item.Many_RightRightRightRight = new List<Association_OneToSingleAndMany_RightRightRightRight>();
                        item.Many_RightRightRightRight.Add(new Association_OneToSingleAndMany_RightRightRightRight());
                        rights4.Add(item.Many_RightRightRightRight[0]);
                    }
                }

                ctx.SaveChanges();
            }
        }
    }
}
