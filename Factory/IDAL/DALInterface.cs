using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    public interface DALInterface<T>
    {
        int InsertBy(T t);
        int DeleteBy(T t);
        int ModifyBy(T t);
        List<T> SelectBy();
        List<T> SelectBy(T t);
    }
}
