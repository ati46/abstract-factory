using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Configuration;

namespace Factory
{
    /// <summary>
    /// DAL父类 --使最基本的增删改查方法进行复用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDAL<T> : DALInterface<T> where T : class, new()
    {
        IDBHelper dbHelper = null;

        public BaseDAL()
        {
            //读取配置文件
            string type = System.Configuration.ConfigurationManager.AppSettings["sqlType"].ToString();
            switch (type)
            {
                case "postgresql":
                    dbHelper = new PostgreHelper();
                    break;
            }
        }
        /*
         *需要注意的地方
         *1. type.GetProperties() 获取的数据位置并不按照一定的顺序排列
         */

        #region 静态
        //获得到泛型的类型(Type)对象
        static Type type = typeof(T);
        //根据得到的对象 获取其全部的公共属性
        static PropertyInfo[] info = type.GetProperties();
        //根据得到的 Type 对象 实力化一个泛型对象  --使用指定类型的默认构造函数来创建该类型的实例
        //static T idal = Activator.CreateInstance(c) as T; 
        #endregion

        #region 新增数据  --InsertBy(T t)
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int InsertBy(T t)
        {
            int result = 0;
            StringBuilder sb = new StringBuilder();
            string[] props = GetInsertNames(info);
            SqlParameter[] pars = GetParameters(t, props);
            sb.Append("insert into ").Append(type.Name).Append("(").Append(string.Join(",", props)).Append(") values(@").Append(string.Join(",@", props)).Append(")");
            //result = SqlHelper.ExecuteNonQuery(sb.ToString(), pars);
            result = dbHelper.ExecuteNonQuery(CommandType.Text, sb.ToString(), pars);
            return result;
        }
        #endregion

        #region 根据条件删除数据  --DeleteBy(T t)
        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int DeleteBy(T t)
        {

            int result = 0;
            StringBuilder sb = new StringBuilder();
            string[] props = GetWhereNames(info, t);
            SqlParameter[] pars = GetParameters(t, props);
            string[] strWheres = GetWheres(props);
            sb.Append("delete ").Append(type.Name).Append(" where ").Append(string.Join(" and ", strWheres));
            //result = SqlHelper.ExecuteNonQuery(sb.ToString(), pars);
            result = dbHelper.ExecuteNonQuery(CommandType.Text, sb.ToString(), pars);
            return result;
        }
        #endregion

        #region 更新数据  --ModifyBy(T t)
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int ModifyBy(T t)
        {
            int result = 0;
            StringBuilder sb = new StringBuilder();
            string[] props = GetModifyNames(info);
            SqlParameter[] pars = GetParameters(t, props);
            string strWheres = GetModifyWheres(props);
            sb.Append("update ").Append(type.Name).Append(" set ").Append(strWheres);
            //result = SqlHelper.ExecuteNonQuery(sb.ToString(), pars);
            result = dbHelper.ExecuteNonQuery(CommandType.Text, sb.ToString(), pars);
            return result;
        }
        #endregion

        #region 查找全部数据  --SelectBy()
        /// <summary>
        /// 查找全部数据
        /// </summary>
        /// <returns></returns>
        public List<T> SelectBy()
        {
            List<T> list = new List<T>();
            StringBuilder sb = new StringBuilder();
            string[] props = GetModifyNames(info);
            sb.Append("select ").Append(string.Join(",", props)).Append(" from ").Append(type.Name);
            //DataTable table = SqlHelper.ExecuteDataTable(sb.ToString(), null);
            DataTable table = dbHelper.ExecuteQuery(CommandType.Text, sb.ToString(), null).Tables[0];
            foreach (DataRow row in table.Rows)
            {
                list.Add(GetDataRow(row));
            }
            return list;
        }
        #endregion

        #region 根据传进来的条件查数据  --SelectBy(T t)
        /// <summary>
        /// 根据传进来的条件查数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<T> SelectBy(T t)
        {
            List<T> list = new List<T>();
            StringBuilder sb = new StringBuilder();
            string[] props = GetWhereNames(info, t);
            SqlParameter[] pars = GetParameters(t, props);
            string[] strWheres = GetWheres(props);
            string[] names = GetModifyNames(info);
            sb.Append("select ").Append(string.Join(",", names)).Append(" from ").Append(type.Name).Append(" where ").Append(string.Join(" and ", strWheres));
            //DataTable table = SqlHelper.ExecuteDataTable(sb.ToString(), pars);
            DataTable table = dbHelper.ExecuteQuery(CommandType.Text, sb.ToString(), pars).Tables[0];
            foreach (DataRow row in table.Rows)
            {
                list.Add(GetDataRow(row));
            }
            return list;
        }
        #endregion


        /*以下方法为实现根据泛型对象来动态生成可以复用的代码*/

        #region 得到属性名的集合(不包括ID) --GetInsertNames(PropertyInfo[] info)
        /// <summary>
        /// 得到属性名的集合(不包括ID)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string[] GetInsertNames(PropertyInfo[] info)
        {
            string[] props = new string[info.Length - 1];
            for (int i = 1; i < info.Length; i++)
            {
                props[i - 1] = info[i].Name;
            }
            return props;
        }
        #endregion

        #region 动态创建 SqlParameter  --GetParameters(T t, params string[] names)
        /// <summary>
        /// 动态创建 SqlParameter
        /// </summary>
        /// <param name="t"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        private static SqlParameter[] GetParameters(T t, params string[] names)
        {
            SqlParameter[] pars = new SqlParameter[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                //GetValue() 需要的是一个传进来的对象，而不是一个类型变量
                pars[i] = new SqlParameter(("@" + names[i]), UtilHelper.ToDbValue(type.GetProperty(names[i]).GetValue(t, null)));
            }
            return pars;
        }
        #endregion

        #region 获取到删除的所有条件  --GetWhereNames(PropertyInfo[] info, T t)
        /// <summary>
        /// 获取到删除的所有条件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string[] GetWhereNames(PropertyInfo[] info, T t)
        {
            //默认给15个初始空间 这样数组就不需要临时扩容 加快性能
            ArrayList list = new ArrayList(15);
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].GetValue(t, null) != null)
                {
                    list.Add(info[i].Name);
                }
            }
            string[] props = new string[list.Count];
            for (int j = 0; j < list.Count; j++)
            {
                props[j] = list[j].ToString();
            }
            return props;
        }
        #endregion

        #region 拼接出删除的Sql语句的Where条件  --GetWheres(string[] strs)
        /// <summary>
        /// 拼接出删除的Sql语句的Where条件
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private static string[] GetWheres(string[] strs)
        {
            string[] strWheres = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                strWheres[i] = strs[i] + " = @" + strs[i];
            }
            return strWheres;
        }
        #endregion

        #region 得到属性名的集合 --GetModifyNames(PropertyInfo[] info)
        /// <summary>
        /// 得到属性名的集合
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string[] GetModifyNames(PropertyInfo[] info)
        {
            string[] props = new string[info.Length];
            for (int i = 0; i < info.Length; i++)
            {
                props[i] = info[i].Name;
            }
            return props;
        }
        #endregion

        #region 拼接出更新的Sql语句的Where条件  --GetModifyWheres(string[] strs)
        /// <summary>
        /// 拼接出更新的Sql语句的Where条件
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private static string GetModifyWheres(string[] strs)
        {
            string[] strWheres = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                strWheres[i] = strs[i] + " = @" + strs[i];
            }
            string props = null;
            string id = strs[0];
            props = string.Join(",", strWheres.Reverse());
            int index = props.LastIndexOf(",");
            props = props.Remove(index, 1);
            props = props.Insert(index, " where ");
            return props;
        }
        #endregion

        #region 使用指定类型的默认构造函数来创建该类型的实例然后根据传入的DataRow 给这个实例对象赋值  --GetDataRow(DataRow row)
        /// <summary>
        /// 使用指定类型的默认构造函数来创建该类型的实例然后根据传入的DataRow 给这个实例对象赋值
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static T GetDataRow(DataRow row)
        {
            //T t = new T();
            //T t = Activator.CreateInstance(type) as T;
            T t = Activator.CreateInstance<T>();
            string[] names = GetModifyNames(info);
            for (int i = 0; i < names.Length; i++)
            {
                PropertyInfo proInfo = type.GetProperty(names[i]);
                proInfo.SetValue(t, UtilHelper.FromDbValue(row[(names[i])]), null);
            }
            return t;
        }
        #endregion

        #region 参考
        //private static string[] GetUpdateColumns(DataTable table)
        //{
        //    string[] names = new string[table.Columns.Count - 1];
        //    for (int i = 1; i < table.Columns.Count; i++)
        //    {
        //        DataColumn dataCol = table.Columns[i];
        //        names[i - 1] = dataCol.ColumnName + " = @" + dataCol.ColumnName;
        //    }
        //    return names;
        //}

        //private static string[] GetAllColumns(DataTable table)
        //{
        //    string[] names = new string[table.Columns.Count];
        //    for (int i = 0; i < table.Columns.Count; i++)
        //    {
        //        DataColumn dataCol = table.Columns[i];
        //        names[i] = dataCol.ColumnName;
        //    }
        //    return names;
        //}

        //private static string[] GetSelectPar(DataTable table)
        //{
        //    string[] pars = new string[table.Columns.Count];
        //    for (int i = 0; i < table.Columns.Count; i++)
        //    {
        //        pars[i] = "\r\n" + table.Columns[i].ColumnName + " = (" + GetDataTypeName(table.Columns[i]) + ")row[\"" + table.Columns[i].ColumnName + "\"]";
        //    }
        //    return pars;
        //} 
        #endregion

    }
}
