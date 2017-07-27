using Common;
using DataStore;
using IDataStore;
using PISLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSolderService
{
    public class AutosolderServiceFun
    {
        public AutosolderServiceFun()
        {
            ExcuteFun();
        }

        List<ExecuteQueue> Executelist;
        private void ExcuteFun()
        {
            ILDO ini = new INI();
            ini.LDOPath = "D:\\AutosolderNet\\AutoSolderNetCon.ini";
            List<NetConfig> listNet = ini.ReadFun();
            if (listNet == null)
            {
                // PISTrace.WriteStrLine("listNet is null");
                LogClass.WriteLogFile("class-AutosolderServiceFun;Fun-ExcuteFun;  :listNet isNull 28行处");
                return;
            }
            Executelist = new List<ExecuteQueue>();
            for (int i = 0; i < listNet.Count; i++)
            {
                AutoSolderService.ExecuteQueue queue = new AutoSolderService.ExecuteQueue();
                int temp;
                try
                {
                    temp = int.Parse(listNet[i].Port);
                }
                catch (Exception ex)
                {

                    temp = 1234;
                    LogClass.WriteLogFile("Class-AutosolderServic;Fun-ExcuteFun; : 44行" + ex.Message);
                }

                queue.setupConection(listNet[i].Ip, temp);
                queue.ExecuteAddBaseProfile(listNet[i].Line);//10s
                Executelist.Add(queue);
                //事件
                IOperationBase Ids = new DataStoreBase();
                Ids.SettingEventScheduler("60", listNet[i].Line);
                
            }
        }
       

        public void closeThread()
        {
            if (Executelist != null && Executelist.Count > 0)
            {
                foreach (ExecuteQueue que in Executelist)
                {
                    que.closeNetWork();
                }
            }
        }
    }
}
