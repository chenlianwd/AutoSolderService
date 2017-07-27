using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IDataStore;

namespace DataStore
{
    public class DataStoreBase : IOperationBase
    {
       
        /// <summary>
        /// 建表以及新增数据
        /// </summary>
        /// <param 数据对象="baseProfile"></param>
        /// <returns></returns>
        public result AddBaseProfile(BaseProfile baseProfile,string tableName)
        {
            if (baseProfile == null)
            {
                return result.fail;
            }
            //在这之前先判断是否安装了mysql数据库，并作出相应提示
            if (!AccessDBBase.ExistMysqlDB())
            {
                return result.notFoundMySql;
            }
            if (AccessDBApply.CreateTable(tableName))
            {
                if (AccessDBApply.InsertBaseProfile(baseProfile.Temperature, baseProfile.Humidity,baseProfile.ProductLine, baseProfile.remainSolderPercent, baseProfile.usedSolderNum, baseProfile.addTimes,  baseProfile.TimePoint, tableName))
                {
                    return result.success;
                }
                else
                {
                    return result.fail;
                }
            }
            else
            {
                return result.fail;
            }
           
        }
        

        /// <summary>
        /// 查询历史记录（单条）
        /// </summary>
        /// <param 查询时间点="timePoint"></param>
        /// <param 返回数据对象="baseprofile"></param>
        /// <returns></returns>
        public bool ReadBaseProfile(string timePoint, out BaseProfile baseprofile)
        {
            baseprofile = AccessDBApply.select_baseprofile(timePoint);
            if (baseprofile == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 查询历史记录（可选范围）返回对象list
        /// </summary>
        /// <param 起始时间点="startTimePoint"></param>
        /// <param 结束时间点="endTimePoint"></param>
        /// <param 返回数据对象数组="baseProfileGroup"></param>
        /// <returns></returns>
        public bool ReadBaseProfile_list(string startTimePoint, string endTimePoint, out List<BaseProfile> baseProfileGroup)
        {
            baseProfileGroup = AccessDBApply.selectList_baseprofile(startTimePoint, endTimePoint);
            if (baseProfileGroup == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 查询历史记录（可选范围）返回DataTable
        /// </summary>
        /// <param 起始时间点="startTimePoint"></param>
        /// <param 结束时间点="endTimePoint"></param>
        /// <param 返回数据="Dt"></param>
        /// <returns></returns>
        public bool ReadBaseProfile_dataTable(string startTimePoint, string endTimePoint, out DataTable Dt)
        {
            Dt = AccessDBApply.selectDataTable_baseprofile(startTimePoint, endTimePoint);
            if (Dt == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 清除某个时间点的数据
        /// </summary>
        /// <param 时间点="timePoint"></param>
        /// <returns></returns>
        public bool RemoveBaseProfile(string starttime, string endtime, string tableName)
        {
            if (AccessDBApply.deleteData_TimeToTime(starttime, endtime, tableName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 清空所有历史数据
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllData(string tableName)
        {
            if (AccessDBApply.DropTable(tableName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 开启事件调度器,在调用时务必设置全局bool值让他只执行一次，另可在设置中重置bool值
        /// </summary>
        /// <param 时间范围="timerange"></param>
        /// <returns></returns>
        public bool SettingEventScheduler(string timerange, string tableName)
        {
            if (AccessDBApply.AutoCleanData(timerange, tableName))
            {
                return true;
            }
            else
            {
                return false;
            }
            

        }

       
    }
}
