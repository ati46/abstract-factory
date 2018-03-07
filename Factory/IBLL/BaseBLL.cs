using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory
{
    /// <summary>
    /// BLL父类 --使最基本的增删改查方法进行复用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseBLL<T> : BLLInterface<T> where T : class,new()
    {
        //protected BaseDAL<T> dal = null;
        protected DALAbsFactory<T> dalAbsFactory = DALAbsFactory<T>.GetFactory();
        protected DALInterface<T> dal = null;
        //使用一个抽象方法让子类在实现父类的时候给出 指向的对象
        public abstract void SetDAL();
        public BaseBLL()
        {
            SetDAL();
        }
        public bool InsertBy(T t)
        {
            return dal.InsertBy(t) == 1 ? true : false;
        }
        public bool DeleteBy(T t)
        {
            return dal.DeleteBy(t) == 1 ? true : false;
        }
        
        public bool ModifyBy(T t)
        {
            return dal.ModifyBy(t) == 1 ? true : false;
        }
        public List<T> SelectBy()
        {
            return dal.SelectBy();
        }
        public List<T> SelectBy(T t)
        {
            return dal.SelectBy(t);
        }
    }
}
