using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using PISLog;
using System.Data;
using System.Reflection;
using Common;

namespace DataStore
{
    public class AccessDBBase
    {
        private static DbConfig dbConfig = new DbConfig();
        /// <summary>
        /// 判断是否安装数据库
        /// </summary>
        /// <returns></returns>
        public static bool ExistMysqlDB()
        {
            bool issuc = true;

            try
            {
                string rootmysqlStr = "Data Source=localhost;User Id=" + dbConfig.User + "; Password=" + dbConfig.Password +";pooling=false;CharSet=utf8;port=" + dbConfig.Port + ";";
                MySqlConnection mycon = new MySqlConnection(rootmysqlStr);
                mycon.Open();
                mycon.Close();
            }
            catch (MySqlException e)
            {
                //捕获到无法建立连接即未安装数据库
                issuc = false;
                //PISTrace.WriteStrLine("ExistMysqlDB" + e.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-ExistMySqlDB;行号35" + e.Message);
                //throw;
            }
            return issuc;
        }
        /// <summary>
        /// 是否存在数据库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        protected static bool ExistDB(string dbname, string user, string pwd, int port)
        {
            bool issuc = true;

            //MySqlErrorCode error = MySqlErrorCode.Yes;

            try
            {
                string rootmysqlStr = "Data Source=localhost;User Id=" + user + "; Password=" + pwd + "; " + "pooling=false;CharSet=utf8;port=" + port.ToString() + ";";
                MySqlConnection mycon = new MySqlConnection(rootmysqlStr);
                mycon.Open();
                MySqlCommand mycmd = new MySqlCommand("show databases", mycon);
                mycmd.ExecuteNonQuery();

                MySqlDataReader myreader = mycmd.ExecuteReader();
                List<string> dbnameg = new List<string>();
                while (myreader.Read())
                {
                    string str = myreader.GetString(0).Trim();
                    if ((str != null) && (str.Length > 0))
                        dbnameg.Add(str);
                }
                mycon.Close();

                if (!dbnameg.Contains(dbname))
                {
                    issuc = false;
                }
            }
            catch (MySqlException e)
            {
                issuc = false;
                //PISTrace.WriteStrLine("ExistDB     :" + e.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-ExistDB;行号81" + e.Message);
                //throw;
            }

            return issuc;
        }
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>是否创建成功</returns>
        protected static bool CreateDB(string dbname, string user, string pwd)
        {
            bool issuc = true;

            try
            {
                string connstr = "Data Source=localhost;Persist Security Info=yes;User Id=" + user + "; PWD=" + pwd + ";";
                MySqlConnection conn = new MySqlConnection(connstr);
                MySqlCommand cmd = new MySqlCommand("CREATE DATABASE " + dbname + ";", conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySqlException e)
            {
                issuc = false;
                //PISTrace.WriteStrLine("CreateDB"+e.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-CreateDB;行号112;" + e.Message);
                throw;
            }

            return issuc;
        }
        /// <summary>
        /// 建立mysql数据库链接
        /// </summary>
        /// <returns></returns>
        protected static MySqlConnection getMySqlCon(string dbname, string user, string pwd, int port)
        {
            String mysqlStr = "Database=" + dbname + ";Data Source=localhost;User Id=" + user + "; PWD=" + pwd + ";pooling=false;CharSet=utf8;port=" + port.ToString();
            MySqlConnection mysql = new MySqlConnection(mysqlStr);
            return mysql;
        }
        /// <summary>
        /// 判断是否存在指定的表
        /// </summary>
        /// <param name="conn">所连接的数据库</param>
        /// <param name="tablename">表名称</param>
        /// <returns></returns>
        protected static bool ExistTable(string dbname, string user, string pwd, int port, string tablename)
        {
            bool issuc = true;

            try
            {
                using (MySqlConnection conn = getMySqlCon(dbname, user, pwd, port))
                {
                    conn.Open();

                    MySqlCommand mycmd = new MySqlCommand("show tables", conn);
                    mycmd.ExecuteNonQuery();

                    MySqlDataReader myreader = mycmd.ExecuteReader();
                    List<string> tbnameg = new List<string>();
                    while (myreader.Read())
                    {
                        string str = myreader.GetString(0).Trim();
                        if ((str != null) && (str.Length > 0))
                            tbnameg.Add(str);
                    }

                    if (!tbnameg.Contains(tablename))
                    {
                        issuc = false;
                    }
                }
            }
            catch (MySqlException e)
            {
                issuc = false;
               // PISTrace.WriteStrLine(e.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-ExistTable;行号166;" + e.Message);
                //throw;
            }

            return issuc;
        }
        /// <summary>
        /// 执行sql命令
        /// </summary>
        /// <param 数据库名="dbname"></param>
        /// <param 用户名="user"></param>
        /// <param 密码="pwd"></param>
        /// <param 端口号="port"></param>
        /// <param sql命令="command"></param>
        /// <returns></returns>
        protected static bool ExecuteCommand(string dbname, string user, string pwd, int port, string command)
        {
            bool issuc = true;

            try
            {
                using (MySqlConnection conn = getMySqlCon(dbname, user, pwd, port))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(command, conn);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {

                issuc = false;
                //PISTrace.WriteStrLine(e.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-ExcuteCommand;行号201;" + e.Message);
                //throw;
            }

            return issuc;
        }
        /// <summary>
        /// 批量事务处理
        /// </summary>
        /// <param sql命令集="SQLStringList"></param>
        public static void ExecuteSqlTransaction(List<string> SQLStringList)
        {
            string rootmysqlStr = "Database=profile; Data Source=localhost;User Id=root; Password=pempenn;pooling=false;CharSet=utf8;port=" + GlobalData.MySqlPort;
            using (MySqlConnection connection = new MySqlConnection(rootmysqlStr))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();//isolationlevel 指定事务的隔离级别。

                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    //......
                    foreach (string sql in SQLStringList)
                    {
                        if (!string.IsNullOrEmpty(sql))
                        {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (MySqlException ex)
                {
                    transaction.Rollback();
                    //PISTrace.WriteStrLine("批量事务处理出错：" + ex.Message);
                    LogClass.WriteLogFile("Class-AccessDBBase;Fun-ExecuteSqlTransaction;行号240;" + ex.Message);
                   // throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }
        //
        /// <summary>
        /// 查询结果集并返回datatable
        /// </summary>
        /// <param MySqlConnection="connection"></param>
        /// <param sql语句="sqlCmd_select"></param>
        /// <param sql参数="cmdParams"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(MySqlConnection connection, string sqlCmd_select, params MySqlParameter[] cmdParams)
        {
            DataTable Dt = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd_select);
                cmd.Connection = connection;
                if (cmdParams != null && cmdParams.Length > 0)
                {
                    cmd.Parameters.AddRange(cmdParams);
                }
                MySqlDataAdapter Da = new MySqlDataAdapter(cmd);
                
                try
                {
                    Da.Fill(Dt);
                }
                catch (MySqlException ex)
                {
                    Dt = null;
                    //PISTrace.WriteStrLine("DataTable填充数据异常" + ex.Message);
                    LogClass.WriteLogFile("Class-AccessDBBase;Fun-GetDataTable;行号281;" + ex.Message);
                   // throw;
                }
            }
            catch (MySqlException ex)
            {
                Dt = null;
               // PISTrace.WriteStrLine("DataTable填充数据异常" + ex.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-GetDataTable;行号289;" + ex.Message);
                //throw;
            }
            return Dt;
        }


        /// <summary>
        /// 通过反射把数据库中的表打包成List<T>对象 
        /// </summary>
        /// <typeparam 返回类型="T"></typeparam>
        /// <param 参数类型="claseeType"></param>
        /// <param 查询sql语句="sqlCmd_select"></param>
        /// <returns></returns>
        public static List<T> MakeTableToPackage<T>(T t, MySqlConnection connection, string sqlCmd_select, params MySqlParameter[] cmdParams) where T : class, new()//where T:class,new() 表示类型T必须是类，并且可以实例化
        {
            List<T> _lstReturn = new List<T>();

            try
            {

                //DataTable _dtGet = SqlHelper.ExcueteDataTable(sqlCmd_select, cmdParams);
                MySqlCommand cmd = new MySqlCommand(sqlCmd_select);
                cmd.Connection = connection;
                if (cmdParams != null && cmdParams.Length > 0)
                {
                    cmd.Parameters.AddRange(cmdParams);
                }
                MySqlDataAdapter Da = new MySqlDataAdapter(cmd);
                DataTable Dt = new DataTable();
                try
                {
                    Da.Fill(Dt);
                }
                catch (MySqlException ex)
                {
                    //PISTrace.WriteStrLine("DataTable填充数据异常" + ex.Message);
                    LogClass.WriteLogFile("Class-AccessDBBase;Fun-MakeTableToPackage;行号326;" + ex.Message);
                    //throw;
                }
                //获得属性集合
                T tmpObj = new T();
                Type type = tmpObj.GetType();
                PropertyInfo[] properties = type.GetProperties();


                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    T item = new T();
                    foreach (PropertyInfo property in properties)
                    {

                        object value = Dt.Rows[i][property.Name];
                        property.SetValue(item,
                            value
                            , null);
                    }
                    _lstReturn.Add(item);
                }

            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine("打包数据出错-MakeTablePackage" + ex.Message);
                LogClass.WriteLogFile("Class-AccessDBBase;Fun-打包数据出错-MakeTablePackage" + ex.Message);
               // throw;

            }
            return _lstReturn;
        }
    }
}
