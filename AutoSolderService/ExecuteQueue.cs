

using AutoSolderService;
using Common;
using DataStore;
using IDataStore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
namespace AutoSolderService
{
    public class ExecuteQueue
    {
        public ExecuteQueue()
        {
            //setupConection();
        }
        //网络通讯类
        NetWorkServer netWorkServer;
        
        BaseProfile baseProfile = null;
        private NetConnectStat netstat;
        public NetConnectStat Netstat
        {
            get
            {
                return netstat;
            }

            set
            {
                netstat = value;
                if (netstat == NetConnectStat.Connect)
                {
                    //T1.Start();
                    T2.Start();
                    
                }
                else
                {
                   // T1.Stop();
                    T2.Stop();
                }
            }
        }
        string Ip;

        public void setupConection(string ip, int port)
        {
            try
            {
                Ip = ip;
                //string IPString = ConfigurationManager.AppSettings["machineip"];
                //int port = 0;
                //int.TryParse(ConfigurationManager.AppSettings["machineport"], out port);
                //string productLine = ConfigurationManager.AppSettings["productline"];          
                //netWorkServer = NetWorkServer.CreateInstance(ip, port);
                netWorkServer = new NetWorkServer(ip, port);
                NetConnectStat stu = netWorkServer.NetStat;
                netWorkServer.NetConnectChanged += NetWorkServer_NetConnectChanged;
                netWorkServer.ReciveDataEvent += NetWorkServer_ReciveDataEvent;
                // DisplayDataEvent += ExecuteQueue_DisplayDataEvent;
            }
            catch (Exception e)
            {
                LogClass.WriteLogFile("Class-ExecuteQueue;Fun-setupConnection; 行号67:" + e.Message);
                //throw;
            }

            
        }
        public void closeNetWork()
        {
            if (netWorkServer != null)
            {
                netWorkServer.close();

            }
            EnableThread = false;
        }
        //private void ExecuteQueue_DisplayDataEvent(object sender, DisplayToUIArgs e)
        //{
        //    OnDisplayDataEvent(e);
          
        //}

        private void NetWorkServer_ReciveDataEvent(object sender, ReciveDataArgs e)
        {

            HandleReciveDataEvent(e.ReciveData);
#warning test---------------
           // Console.WriteLine(e.ReciveData.ToString() + DateTime.Now + Ip);
        }
       
        private void HandleReciveDataEvent(byte[] s)
        {
            BaseProfile _baseprofile = new BaseProfile();
            _baseprofile.Temperature = (s[0] + (s[1] << 8)) / 10.0;//温度
            _baseprofile.Humidity = s[2] + (s[3] << 8);//湿度
            _baseprofile.remainSolderPercent = s[4] + (s[5] << 8);//剩余量
            _baseprofile.usedSolderNum = s[6] + (s[7] << 8);//已使用瓶数
            _baseprofile.addTimes = s[8] + (s[9] << 8);//次数
            _baseprofile.startTime = s[10] + (s[11] << 8);//启动次数
            _baseprofile.powerOffTime = s[12] + (s[13] << 8);//开机时间

            _baseprofile.TimePoint = DateTime.Now;//数据点时间

#warning test---------------
            //_baseprofile.Temperature = 25.5d;
            //_baseprofile.Humidity = 55.92d;
            //_baseprofile.remainSolderPercent = 90;
            //_baseprofile.usedSolderNum = 999;
            //_baseprofile.addTimes = 666;
            //_baseprofile.startTime = 1;
            //_baseprofile.powerOffTime = 1;
            //Console.Write("温度" + _baseprofile.Temperature.ToString() + "湿度" + _baseprofile.Humidity + "剩余量" + _baseprofile.remainSolderPercent + "已用瓶数" + _baseprofile.usedSolderNum + "次数" + _baseprofile.addTimes + "启动时间" + _baseprofile.startTime + "开机时间" + _baseprofile.powerOffTime);


            _baseprofile.ProductLine = TableName;
            baseProfile = _baseprofile;
        }
        private void NetWorkServer_NetConnectChanged(object sender, NetConnectArgs e)
        {
            Netstat = e.NetStat;
            OnNetConnectChanged(e);
            //Console.WriteLine(e.NetStat.ToString() + Ip);
        }
        
        //save date queue and show UI queue
        private Queue<BaseProfile> baseProfileQueue_save = new Queue<BaseProfile>();
        private Queue<BaseProfile> baseProfileQueue_show = new Queue<BaseProfile>();
        
        // 通过 _wh 给工作线程发信号
        static EventWaitHandle _whsv = new AutoResetEvent(false);
        static EventWaitHandle _whsw = new AutoResetEvent(false);
         Thread _saveWorker;
        //static Thread _showWorker;
        //System.Timers.Timer T1 = new System.Timers.Timer();//显示的频率
        System.Timers.Timer T2 = new System.Timers.Timer(10000);//存入数据库的频率10s

        string TableName;
        public  void ExecuteAddBaseProfile( string tableName)
        {
            try
            {
                TableName = tableName;

                T2.Elapsed += T2_Elapsed;
                //T2.Interval = timer * 1000;
                _saveWorker = new Thread(saveWork);
                _saveWorker.IsBackground = true;
                _saveWorker.Name = tableName;
                _saveWorker.Start();
            }
            catch (Exception e)
            {
                LogClass.WriteLogFile("Class-ExecuteQueue;Fun-ExecuteAddBaseProfile; 行号157:" + e.Message);
                //throw;
            }
            //T1.Interval = timer * 1000;
            //T1.Elapsed += T1_Elapsed;
           

           // Dispose_save();

            //_showWorker = new Thread(showWork);
            //_showWorker.IsBackground = true;
            //_showWorker.Start();
           // Dispose_show();
        }      
        public void updateIntervalTime(int time)
        {
           
           // T1.Interval = time * 1000;
        }
        private bool EnableThread = true;

        private void saveWork()
        {
            while (EnableThread)
            {
                try
                {
                    
                        if (baseProfileQueue_save.Count > 0)
                        {
                            BaseProfile baseData = baseProfileQueue_save.Dequeue(); // 有任务时，出列任务

                            if (baseData == null)  // 退出机制：当遇见一个null任务时，代表任务结束
                                return;
                            else
                            SaveData(baseData, _saveWorker.Name);  // 任务不为null时，处理并保存数据
                         }
                    
                }
                catch (Exception e)
                {
                    //PISLog.PISTrace.WriteStrLine("savework：" + e.Message);
                    LogClass.WriteLogFile("Class-AutosolderService;Fun-saveWork; 198行 " + e.Message);
                }
                Thread.Sleep(300); 
            }
        }
        //private void showWork()
        //{
        //    while (true)
        //    {                
        //        //lock (this)
        //        {
        //            if (baseProfileQueue_show.Count > 0)
        //            {
        //                BaseProfile baseData = baseProfileQueue_show.Dequeue(); // 有任务时，出列任务

        //                if (baseData == null)  // 退出机制：当遇见一个null任务时，代表任务结束
        //                    return;
                        
        //                    OnDisplayDataEvent(new DisplayToUIArgs(baseData));//传给主窗体
                       
                        

        //            }
        //            //else
        //            //    _whsw.WaitOne();   // 没有任务了，等待信号
        //        }
            
                   
        //    }
        //}

       


        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
           // lock (this)
            {
                if (baseProfile != null)
                {
                    baseProfileQueue_show.Enqueue(baseProfile);
                  //  _whsv.Set();// 给工作线程发信号
                }
                
            }
        }
        private void T2_Elapsed(object sender, ElapsedEventArgs e)
        {
           // lock (this)
            {
                if (baseProfile != null)
                {
                    baseProfileQueue_save.Enqueue(baseProfile);
                   // _whsw.Set();// 给工作线程发信号
                }
               
            }
        }

       
        /// <summary>
        /// 保存到数据库
        /// </summary>
        /// <param name="baseprofile"></param>
        private void SaveData(BaseProfile baseprofile, string tableName)
        {
            DataStoreBase dataStoreBase = new DataStoreBase();

            dataStoreBase.AddBaseProfile(baseprofile, tableName);

        }
       
        #region custom define event

        public event NetConnectHandler NetConnectChanged;

        private void OnNetConnectChanged(NetConnectArgs e)
        {
            if (this.NetConnectChanged != null)
                this.NetConnectChanged(new object(), e);
        }

       

        #endregion
        

    }
}
