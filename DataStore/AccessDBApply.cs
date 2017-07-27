using IDataStore;
using MySql.Data.MySqlClient;
using PISLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common;
namespace DataStore
{
    class AccessDBApply:AccessDBBase
    {

       private static DbConfig dbConfig = new DbConfig();
        //建表
        /// <summary>
        /// Create Table
        /// </summary>
        /// <returns></returns>
        public static bool CreateTable(string tableName)
        {
            bool issuc = true;
            try
            {
               
                //**判断数据库是否存在，以及数据库是否创建成功*/
                if (!ExistDB(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port)))
                {
                    if (!CreateDB(dbConfig.Catalog, dbConfig.User, dbConfig.Password))
                        return false;
                }
                //**建表*/
                if (!ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port),sqlCmd_baseProfile(tableName)))
                {
                    // PISTrace.WriteStrLine("创建表baseprofile失败");
                    LogClass.WriteLogFile("Class-AcessDBApply;Fun-selectDataTable_baseprofile; 37行:" + "创建表baseprofile失败:");
                    return false;
                }

            }
            catch (MySqlException ex)
            {
               // PISTrace.WriteStrLine("CreateTable " +ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-Create; 44行:" + ex.Message);
                return false;
            }
            return issuc;
        }
        //*******************************************************************insert
        /// <summary>
        /// insert into data
        /// </summary>
        /// <param 温度="Temperature"></param>
        /// <param 湿度="Humidity"></param>
        /// <param 生产线="ProductLine"></param>
        /// <param 时间点="TimePoint"></param>
        /// <returns></returns>
        /// 
        public static bool InsertBaseProfile(double Temperature, double Humidity, string ProductLine, int RemainSolderPercent, int UsedSolderNum, int AddTimes,   DateTime TimePoint, string tableName)
        {
            string timePoint = string.Format("{0:yyyy-MM-dd HH:mm:ss}", TimePoint);
            string getCommandInsertInto_Baseprofile = string.Format(baseProfile_insertFormat(tableName), Temperature, Humidity, "'" + ProductLine + "'",RemainSolderPercent, UsedSolderNum, AddTimes,  "'" +timePoint+ "'");
        
            try
            {
                if (ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password, int.Parse(dbConfig.Port), getCommandInsertInto_Baseprofile))
                {
                    return true;
                }
                else
                {
                    //PISTrace.WriteStrLine("baseProfile插入数据失败");
                    LogClass.WriteLogFile("Class-AcessDBApply;Fun-InsertBaseProfile; 73行:" + "插入数据失败");
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine(ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-InsertBaseProfile; 73行:" + "插入数据失败"+ex.Message);
                return false;
            }
        }
        //***************************************************************delete
        //**清除所有历史记录---主动*/
        //即删除表中所有数据
        public static bool DropTable(string tableName)
        {
            try
            {
                //使用drop而不使用truncate的原因是truncate不能跟if exists...
                if (ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port), sqlCmd_dropTable) && CreateTable(tableName))
                {
                    return true;
                }
                else
                {
                    //PISTrace.WriteStrLine("清除所有历史数据失败");
                    LogClass.WriteLogFile("Class-AcessDBApply;Fun-DropTable; 99行:" + "清除所有历史数据失败");
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine(ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-DropTable; 99行:" + "清除所有历史数据失败" + ex.Message);
                return false;
            }
        }

        //**清除某个时间段数据或者某条数据---*/
        public static bool deleteData_TimeToTime(string startTime, string endTime, string tableName)
        {
            try
            {
                if (!ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port), sqlCmd_deleteData_TimeToTime(startTime, endTime, tableName)))
                {
                    //PISTrace.WriteStrLine("清除某个时间段数据失败");
                    LogClass.WriteLogFile("Class-AcessDBApply;Fun-deleteData_TimeToTime; 119行:" + "清除某个时间段数据失败" );
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                // PISTrace.WriteStrLine("清除某个时间段数据失败" + ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-deleteData_TimeToTime; 119行:" + "清除某个时间段数据失败" + ex.Message);
                return false;
            }
            return true;
        }

       

        //**清除超过一段时间的数据（三个月或者多少天之前）---被动/
        //创建定时任务
        /// <summary>
        /// 定时事件
        /// </summary>
        /// <param 设置默认删除多少天前的数据="timeRange"></param>
        public static bool AutoCleanData(string timeRange, string tableName)
        {
            try
            {
                //createtable是防止表不存在
                if (CreateTable(tableName)&&ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port), sqlCmd_event_scheduler)&& ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port), sqlCmd_drop_oldevent(tableName)))
                {
                    if (!ExecuteCommand(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port), AutoDeleteSqlStr(timeRange, tableName)))
                    {
                        //PISTrace.WriteStrLine("定时事件创建失败");
                        LogClass.WriteLogFile("Class-AcessDBApply;Fun-AutoCleanData; 150行:" + "定时事件创建失败");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine("定时事件创建失败:" + ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-AutoCleanData; 167行:" + "定时事件创建失败" + ex.Message);
                return false;
            }
        }
        //*************************************************************select
        //查询单条数据
        public static BaseProfile select_baseprofile(string timePoint)
        {
            BaseProfile baseProfile = new BaseProfile();
            try
            {
                using (MySqlConnection connection = getMySqlCon(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port)))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = sqlCmd_selectOne(timePoint);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                baseProfile.Temperature = reader.GetDouble("Temperature");
                                baseProfile.Humidity = reader.GetDouble("Humidity");
                                baseProfile.ProductLine = reader.GetString("ProductLine");
                                baseProfile.TimePoint = reader.GetDateTime("TimePoint");
                            }
                        }
                        else
                        {
                            baseProfile = null;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine("查询单条数据失败:" + ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-select_baseprofile; 205行:" + "查询单条数据失败" + ex.Message);
                baseProfile = null;
            }

            return baseProfile;
        }

        
        /// <summary>
        /// 查询时间范围内数据,(实际上查询单条时间点的数据也包含在其中了)并返回对象数组
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static List<BaseProfile> selectList_baseprofile(string starttime, string endtime)
        {
            List<BaseProfile> listBaseProfile = new List<BaseProfile>();
           
            try
            {
                using (MySqlConnection connection = getMySqlCon(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port)))
                {
                    connection.Open();
                    //MySqlCommand command = connection.CreateCommand();
                    listBaseProfile = MakeTableToPackage(new BaseProfile(), connection, sqlCmd_selectData_timeToTime, new MySqlParameter("@starttime", starttime), new MySqlParameter("@endtime", endtime));
                }
            }
            catch (MySqlException ex)
            {
                //PISTrace.WriteStrLine("查询时间范围内数据出错:" + ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-selectList_baseprofile; 235行:" + "查询时间范围内数据出错:" + ex.Message);
                listBaseProfile = null;
            }
            return listBaseProfile;
        }
        /// <summary>
        /// 查询时间范围内数据,(实际上查询单条时间点的数据也包含在其中了)并返回DataTable
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static DataTable selectDataTable_baseprofile(string starttime, string endtime)
        {
            DataTable Dt = new DataTable();
            try
            {
                using (MySqlConnection connection = getMySqlCon(dbConfig.Catalog, dbConfig.User, dbConfig.Password,int.Parse(dbConfig.Port)))
                {
                    connection.Open();
                    //MySqlCommand command = connection.CreateCommand();
                    Dt = GetDataTable(connection, sqlCmd_selectData_timeToTime, new MySqlParameter("@starttime", starttime), new MySqlParameter("@endtime", endtime));
                }
            }
            catch (MySqlException ex)
            {
                // PISTrace.WriteStrLine("查询时间范围内数据出错:" + ex.Message);
                LogClass.WriteLogFile("Class-AcessDBApply;Fun-selectDataTable_baseprofile; 261行:" + "查询时间范围内数据出错:" + ex.Message);
                Dt = null;
            }

            return Dt;
        }
        //*****************sqlCmd********************//

        //开启事件调度器
        private static readonly string sqlCmd_event_scheduler = "SET GLOBAL event_scheduler = ON";
        //删除旧事件
       // private static readonly string sqlCmd_drop_oldevent = "drop event if exists auto_delete";
        private static string sqlCmd_drop_oldevent(string tableName)
        {
            return "drop event if exists `auto_delete" + tableName + "`";
        }
        private static string AutoDeleteSqlStr(string timeRange, string tableName)
        {
            string sqlCmd_AutoDelete = "create event `auto_delete" + tableName + "` " +
                "on schedule " +
                "every 1 day starts now() " +
                "ON COMPLETION  PRESERVE ENABLE " +
                "do " +
                "delete from `"+ tableName +"` where timepoint < date_sub(now(), interval " + timeRange + " day)";

            return sqlCmd_AutoDelete;
        }
        private static string sqlCmd_deleteData_TimeToTime(string startTime, string endTime,string tableName)
        {
            string sqlCmd_deleteData = "delete from `"+ tableName +"` where TimePoint between '" + startTime + "' and " + "'" + endTime + "'";
            return sqlCmd_deleteData;
        }

        private static string sqlCmd_baseProfile(string tableName)
        {
            return "CREATE TABLE if not exists `" + tableName + "` (" +
            "id int not null primary key auto_increment," +
            "Temperature double(7,3) not null," +
            "Humidity double(7,3) not null," +
            "ProductLine varchar(25)," +
            "RemainSolderPercent int not null," +
            "UsedSolderNum int not null," +
            "AddTimes int not null," +
            "TimePoint datetime not null, " +
            "INDEX TimePoint_index (TimePoint)" +
            ")";
        }


       
        
        private static string baseProfile_insertFormat(string tableName)
        {
            return "insert into `"+ tableName + "` (" +
            "Temperature, Humidity, ProductLine, RemainSolderPercent, UsedSolderNum, AddTimes, TimePoint)values({0},{1},{2},{3},{4},{5},{6})";
        }
        //private static  string baseProfile_insertFormat = 
        //truncate table删数据不删表结构，delete table一样，且不释放空间，即自增id继续加载后面。
        //drop table删数据且删表结构，删的话就不能在重新建表之前操作了；
        //但是遗憾的是查了很多资料truncate table 后面没办法跟if exists
        //但是有必须要有判断，所以只好用drop后在新建表的办法了。
        //注意：不同的数据库if exists的位置可能不一样
        private static readonly string sqlCmd_dropTable = "drop table if exists baseprofile";


        //这里的语法要注意
        private static string sqlCmd_selectData_timeToTime = "select * from baseprofile where TimePoint >= @starttime and TimePoint <= @endtime";

        private static string sqlCmd_selectOne(string timePoint)
        {
            string sqlCmd_selectone = "select * from baseprofile where TimePoint = '" + timePoint + "'";
            return sqlCmd_selectone;
        }
    }
}
