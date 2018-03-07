using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    /// <summary>
    /// 工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BLLFactory<T> : BLLAbsFactory<T> where T : class,new()
    {
        /// <summary>
        /// 实现抽象工厂中的GetUser()方法
        /// </summary>
        /// <returns></returns>
        public override BLLInterface<T> GetObject<R>()
        {
            return new R() as BaseBLL<T>;
        }
    }
}
