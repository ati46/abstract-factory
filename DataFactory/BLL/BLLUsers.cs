using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Factory;

namespace DataFactory
{
    public class BLLUsers:BaseBLL<Users>
    {
        DALUsers users = null;
        //注释的代码是可以正常访问的，但是不会调用 dal抽象工厂
        //public override void SetDAL()
        //{
        //    dal = new DALUsers();
        //    users = dal as DALUsers;
        //}
        //DALInterface<Users> users = null;
        public override void SetDAL()
        {
            dal = dalAbsFactory.GetObject<DALUsers>();
            users = dal as DALUsers;
        }

        public int getTest()
        {
            return users.getTest();
        }
    }
}
