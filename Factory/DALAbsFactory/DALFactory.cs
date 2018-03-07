using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    public class DALFactory<T> : DALAbsFactory<T>
        where T : class,new()
    {
        public override DALInterface<T> GetObject<R>()
        {
            return new R() as BaseDAL<T>;
        }
    }
}
