using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    public abstract class DALAbsFactory<T> where T : class,new()
    {
        public static DALAbsFactory<T> GetFactory()
        {
            string type = System.Configuration.ConfigurationManager.AppSettings["dalType"].ToString();
            DALAbsFactory<T> dal = null;
            switch (type)
            {
                case "mssql":
                    dal = new DALFactory<T>();
                    break;
                case "postgresql":
                    dal = new DALFactory<T>();
                    break;
            }
            return dal;
        }

        public abstract DALInterface<T> GetObject<R>() where R : class, new();
    }
}
