using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDataStore
{
    public class BaseProfile
    {
        public BaseProfile()
        {
            Temperature = 0.0d;
            Humidity = 0.0d;
            ProductLine = "";
            TimePoint = new DateTime();
        }
        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// 生产线
        /// </summary>
        public string ProductLine { get; set; }

        /// <summary>
        /// 时间点
        /// </summary>
        public DateTime TimePoint { get; set; }


        //**以上四条属性为数据库所存,其他只做显示*/
        //以下所接受的数据均为byte类型，所以用int足够
        /// <summary>
        /// 剩余锡膏百分比
        /// </summary>
        public int remainSolderPercent { get; set; }

        /// <summary>
        /// 已使用锡膏瓶数
        /// </summary>
        public int usedSolderNum { get; set; }

        /// <summary>
        /// 锡膏添加次数
        /// </summary>
        public int addTimes { get; set; }

        /// <summary>
        /// 加锡机添加锡膏启动时间/min
        /// </summary>
        public int startTime { get; set; }

        /// <summary>
        /// 加锡机系统开机时间/min
        /// </summary>
        public int powerOffTime { get; set; }

    }
}
