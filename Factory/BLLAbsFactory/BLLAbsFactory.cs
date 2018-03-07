using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    /// <summary>
    /// 抽象工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BLLAbsFactory<T> where T : class,new()
    {
        /// <summary>
        /// 读取配置文件 获取实体工厂对象
        /// </summary>
        /// <returns></returns>
        public static BLLAbsFactory<T> GetFactory()
        {
            //读取配置文件
            string type = System.Configuration.ConfigurationManager.AppSettings["bllType"].ToString();
            BLLAbsFactory<T> bll = null;
            switch (type)
            {
                case "mssql":
                    bll = new BLLFactory<T>();
                    break;
                case "postgresql":
                    bll = new BLLFactory<T>();
                    break;
            }
            return bll;
        }

        public abstract BLLInterface<T> GetObject<R>() where R : class, new();
    }
}
