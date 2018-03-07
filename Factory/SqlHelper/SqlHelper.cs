using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public static class SqlHelper
{
    //只用加载一次 加载后就不用修改
    private static readonly string connStr = ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;

    //SqlCommand cmd = conn.CreateCommand() 使用此代码创建的 SqlCommand 的对象的 CommandText 属性会自动根据传进的语句来
    //判断是T-Sql语句或者是存储过程或者表名 就不用手动 指定 CommandType 的类型

    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                //foreach (SqlParameter item in parameters)
                //{
                //    cmd.Parameters.Add(item);
                //}
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters); 
                }
                return cmd.ExecuteNonQuery();
            }
        }
    }

    public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }
    }

    //只用来执行查询结果比较少的sql
    public static DataSet ExecuteDataSet(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                //获得表的信息 --一般不写不过加上更好
                sda.FillSchema(ds, SchemaType.Source);
                sda.Fill(ds);
                return ds;
                //使用代码
                //DataSet ds = .....
                //foreach (DataRow item in ds.Tables[0].Rows)
                //{
                //    string name = (string)item["Name"];
                //}
            }
        }
    }

    public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
    {
        //直接从 DataSet 改变过来的写法
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                //获得表的信息 --一般不写不过加上更好
                //sda.FillSchema(ds, SchemaType.Source);
                sda.Fill(ds);
                return ds.Tables[0];
                //使用代码
                //DataTable table = .....
                //foreach (DataRow item in table.Rows)
                //{
                //    string name = (string)item["Name"];
                //}
            }
        }
        //另一种写法
        //SqlDataAdapter sda = new SqlDataAdapter(sql, connStr);
        //if (parameters != null)
        //{
        //    sda.SelectCommand.Parameters.AddRange(parameters);
        //}
        //DataTable dt = new DataTable();
        //sda.Fill(dt);
        //return dt;
    }

    public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameters)
    {
        SqlConnection conn = new SqlConnection(connStr);
        conn.Open();
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = sql;
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            //CommandBehavior.CloseConnection 当用户关闭reader的时候 系统会自动将SqlConnection关闭
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            //返回SqlDataReader时 SqlConnection和DataReader都不能关闭
            return reader;
        }
    }

    
}
