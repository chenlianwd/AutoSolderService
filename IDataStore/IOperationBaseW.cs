using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDataStore
{
    public enum result { fail, success, notFoundMySql };
    public interface IOperationBaseW
    {

        /// <summary>
        /// 向数据库中写入Base Profile
        /// </summary>
        /// <param 数据对象="baseProfile"></param>
        /// <returns></returns>
        result AddBaseProfile(BaseProfile baseProfile, string tableName);
        /// <summary>
        /// 清除某个时间段数据
        /// </summary>
        /// <param 起始时间="starttime"></param>
        /// <param 终止时间="endtime"></param>
        /// <returns></returns>
        bool RemoveBaseProfile(string starttime, string endtime, string tableName);//可能不会用到
        /// <summary>
        /// 清空所有历史数据
        /// </summary>
        /// <returns></returns>
        bool RemoveAllData(string tableName);
    }
}
