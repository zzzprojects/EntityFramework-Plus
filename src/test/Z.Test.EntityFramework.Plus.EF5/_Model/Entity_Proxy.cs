using System.Collections.Generic;

namespace Z.Test.EntityFramework.Plus
{
    public class Entity_Proxy
    {
        public int ID { get; set; }

        public int ColumnInt { get; set; }

        public virtual ICollection<Entity_Proxy_Right> Rights { get; set; }
    }
}