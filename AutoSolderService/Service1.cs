

using Common;
using DataStore;
using IDataStore;
using PISLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutoSolderService
{
    public partial class Service1 : ServiceBase
    {
        //由于文件夹的检测所以会触及到系统安全,程序的权限设置就显得很有必要,避免程序被误认为是非法操作
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public Service1()
        {
            InitializeComponent();

        }
       
        

        // NetWorkServer server;
        AutosolderServiceFun autosolder = null;
        protected override void OnStart(string[] args)
        {
           // LogClass.WriteLogFile("OnStart:服务已启动...");
            autosolder = new AutosolderServiceFun();

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = "D:\\AutosolderNet";
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Changed += (object source, FileSystemEventArgs e) =>
            {
                watcher.EnableRaisingEvents = false;//这句和下面的是防止多次执行
                                                    //autosolder.closeThread();
                                                    // autosolder = new AutosolderServiceFun();

                //string path = "D:\\AutosolderNet\\AutoSolderReStart.bat";
                // ExecuteBatFile(path);
                //RestartService();
                //使服务退出，给windows发不正常退出的消息，然后由已经设置好的自动重启。
                Environment.Exit(1);
                watcher.EnableRaisingEvents = true;
            };
            watcher.EnableRaisingEvents = true;

        }

        //public static bool ExecuteBatFile(string batName)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (Process pro = new Process())
        //        {
        //            FileInfo file = new FileInfo(batName);
        //            pro.StartInfo.WorkingDirectory = file.Directory.FullName;
        //            pro.StartInfo.FileName = batName;
        //            pro.StartInfo.CreateNoWindow = false;
        //            pro.StartInfo.Verb = "runas";
        //            pro.Start();
        //            pro.WaitForExit();
        //        }

        //        result = true;
        //    }
        //    catch (Exception e)
        //    {
        //        //写入日志
        //        PISLog.PISTrace.WriteStrLine(batName);
        //        PISLog.PISTrace.WriteStrLine("executebaterror:" + e.Message);
        //    }
        //    return result;
        //}

        protected override void OnStop()
        {
           
           
        }
        /// <summary>
        /// 此方法重启服务 顺序并不是按照停止再启动，因此换成在部署服务后设置 遇到错误自动重启（而这个错误是程序给的）
        /// </summary>
        private static void RestartService()
        {
          
            ServiceController serv = new ServiceController("Autosolder");
            //if (serv.CanStop)
            //{
            //    // 如果权限不够是不能Stop()的。
            //    serv.Stop();

            //    // 这句话如果没有对该服务的后续操作可以不要，C#程序只是以权限向操作系统发出关闭某服务的消息而已，真正关闭该服务的是操作系统而非此C#程序（下面的Start的也一样）
            //    serv.WaitForStatus(ServiceControllerStatus.Stopped);
            //}

            //if (!serv.CanStop) // 不能停止，反过来就是可以开启
            //{
            //    serv.Start();
            //    serv.WaitForStatus(ServiceControllerStatus.Running);
            //}

            //// 释放对该服务的控制权及释放相应资源。
            //serv.Close();
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(10000);
                if (serv.Status == ServiceControllerStatus.Running)
                {
                    //object lockThis = new object(); 


                    serv.Stop();
                    serv.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    
                   

                    writeTest("是确定的服务关闭");
                }
                if (serv.Status == ServiceControllerStatus.Stopped)
                {
                    serv.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    writeTest("是确定的服务开启");
                }
               

            }
            catch (Exception e)
            {
                //PISTrace.WriteStrLine("restartservice：" + e.Message);
                LogClass.WriteLogFile("Class-AutosolderService;Fun-RestartService; " + e.Message);
            }
            finally
            {
                //*/
                if (serv.Status == ServiceControllerStatus.Stopped)
                {
                    serv.Start();
                    serv.WaitForStatus(ServiceControllerStatus.Running);
                }
                serv.Close();
            }    
        }
        private static void writeTest(string msg)
        {

            StreamWriter sw = new StreamWriter("D:\\1.txt");

            sw.Write(msg);
            sw.Close();
        }
    }
}
