using System.Collections.Generic;

namespace Z.EntityFramework.Plus
{
    public class QueryFilterAlwaysEmptyDictionaryComparer<TKey, TValue> : IEqualityComparer<TKey>
    {
        public Dictionary<TKey, TValue> Dictionary;

        public bool Equals(TKey x, TKey y)
        {
            // It's "emtpty"! so it always false
            return false;
        }

        public int GetHashCode(TKey obj)
        {
            // A dictionary always take the HashCode to retrieve the bucket, so we simply hack this part and pray for GOD no side effect happen!
            // Since the dictionary cannot be used in concurrency scenario (or must be in a lock first!), it's should never cause any issue
            if (Dictionary != null && Dictionary.Count > 0)
            {
                Dictionary.Clear();
            }

            return base.GetHashCode();
        }
    }

    public class QueryFilterAlwaysEmptyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public QueryFilterAlwaysEmptyDictionary(QueryFilterAlwaysEmptyDictionaryComparer<TKey, TValue> comparer) : base(comparer)
        {
            comparer.Dictionary = this;
        }
    }
}