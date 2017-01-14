namespace Z.EntityFramework.Plus.QueryCache
{
    public interface ICacheTagProvider
    {
        void AddOrUpdate(string cacheKey, string fullTag);
        IRemoveTagResult Remove(string fullTag);
    }
}