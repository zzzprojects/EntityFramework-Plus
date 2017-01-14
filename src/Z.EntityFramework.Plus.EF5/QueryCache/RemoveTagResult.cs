using System.Collections.Generic;

namespace Z.EntityFramework.Plus.QueryCache
{
    public class RemoveTagResult : IRemoveTagResult
    {
        public bool Success { get; }
        public IList<string> Items { get; }

        public RemoveTagResult(bool success, IList<string> result)
        {
            Success = success;
            Items = result;
        }
    }
}