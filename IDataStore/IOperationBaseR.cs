using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IDataStore
{
    public interface IOperationBaseR
    {
        /// <summary>
        /// 读取单个Base Profile
        /// </summary>
        /// <param 时间点="timePoint"></param>
        /// <param 数据对象="baseprofile"></param>
        /// <returns></returns>
        bool ReadBaseProfile(string timePoint, out BaseProfile baseprofile);
        /// <summary>
        /// 范围读取Base Profile，返回对象list
        /// </summary>
        /// <param 起始时间="startTimePoint"></param>
        /// <param 结束时间="endTimePoint"></param>
        /// <param 数据集合="baseProfileGroup"></param>
        /// <returns></returns>
        bool ReadBaseProfile_list(string startTimePoint, string endTimePoint, out List<BaseProfile> baseProfileGroup);

        /// <summary>
        /// 范围读取Base Profile，返回DataTable
        /// </summary>
        /// <param 起始时间="startTimePoint"></param>
        /// <param 结束时间="endTimePoint"></param>
        /// <param 数据集合="Dt"></param>
        /// <returns></returns>
        bool ReadBaseProfile_dataTable(string startTimePoint, string endTimePoint, out DataTable Dt);
    }
}
