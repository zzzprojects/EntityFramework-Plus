using System.Collections.Generic;

namespace Z.EntityFramework.Plus.QueryCache
{
    public interface IRemoveTagResult
    {
        bool Success { get; }
        IList<string> Items { get; }
    }
}
