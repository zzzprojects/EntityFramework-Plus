namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for query includes.</summary>
    public class QueryIncludeManager
    {
        /// <summary>Static constructor.</summary>
        static QueryIncludeManager()
        {
            BatchQuery = true;
        }

        /// <summary>Gets or sets a value indicating whether the batch query.</summary>
        /// <value>true if batch query, false if not.</value>
        public static bool BatchQuery { get; set; }
    }
}