namespace Z.EntityFramework.Plus
{
    public class QueryIncludeManager
    {
        static QueryIncludeManager()
        {
            BatchQuery = true;
        }

        public static bool BatchQuery { get; set; }
    }
}