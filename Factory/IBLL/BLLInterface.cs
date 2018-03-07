using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    public interface BLLInterface<T>
    {
        bool InsertBy(T t);
        bool DeleteBy(T t);
        bool ModifyBy(T t);
        List<T> SelectBy();
        List<T> SelectBy(T t);
    }
}
