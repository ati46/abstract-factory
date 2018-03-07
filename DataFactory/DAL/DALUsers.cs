using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Factory;

namespace DataFactory
{
    public class DALUsers :BaseDAL<Users>
    {
        public int getTest()
        {
            return 1;
        }
    }
}
