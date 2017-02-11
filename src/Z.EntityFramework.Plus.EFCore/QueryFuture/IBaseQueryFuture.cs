using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace Z.EntityFramework.Plus
{
    public interface IBaseQueryFuture
    {
        void GetResultDirectly();
        void SetResult(DbDataReader reader);

        IRelationalCommand CreateExecutorAndGetCommand(out RelationalQueryContext queryContext);
    }
}
