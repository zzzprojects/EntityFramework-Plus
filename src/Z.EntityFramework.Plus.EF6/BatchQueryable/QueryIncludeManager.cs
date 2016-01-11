namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for query includes.</summary>
    internal class QueryIncludeManager2
    {
        /// <summary>Static constructor.</summary>
        static QueryIncludeManager2()
        {
            BatchQuery = true;
        }

        /// <summary>Gets or sets a value indicating whether the batch query.</summary>
        /// <value>true if batch query, false if not.</value>
        public static bool BatchQuery { get; set; }
    }
}