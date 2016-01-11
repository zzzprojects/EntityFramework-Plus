using System.Data.Entity.Core.Objects;

namespace Z.EntityFramework.Plus
{
    public interface IQueryIncludeQuery
    {
        ObjectQuery GetObjectQuery(object orginalQuery);
    }
}