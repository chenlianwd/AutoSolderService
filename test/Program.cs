


using AutoSolder.BLL;
using AutoSolderService;
using Common;
using DataStore;
using IDataStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    class Program
    {

        public static bool ExecuteBatFile(string batName)
        {
            bool result = false;
            try
            {
                using (Process pro = new Process())
                {
                    FileInfo file = new FileInfo(batName);
                    pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                    pro.StartInfo.FileName = batName;
                    pro.StartInfo.CreateNoWindow = true;
                    pro.StartInfo.Verb = "runas";
                   // pro.StartInfo.RedirectStandardInput = true;
                    //pro.StartInfo.RedirectStandardOutput = true;
                   // pro.StartInfo.RedirectStandardError = true;
                   //pro.StartInfo.UseShellExecute = false;
                   //pro.StandardInput.AutoFlush = true;
                    pro.Start();
                    pro.WaitForExit();
                }

                result = true;
            }
            catch (Exception e)
            {
                //写入日志
                throw;
            }
            return result;
        }


        static void Main(string[] args)
        {

            File.Move(@"D:\\AutosolderNet\\LOGFile.txt", @"D:\\AutosolderNet\\" + DateTime.Now.ToString("yyyyMMdd") + "LogFile.txt");
            //try
            //{
            //    Console.WriteLine("start");


            //    AutosolderServiceFun fun = null;

            //    fun = new AutosolderServiceFun();

            //    FileSystemWatcher watcher = new FileSystemWatcher();
            //    watcher.Path = "D:\\AutosolderNet";
            //    watcher.NotifyFilter = NotifyFilters.LastWrite;

            //    watcher.Changed += (object source, FileSystemEventArgs e) =>
            //    {
            //        watcher.EnableRaisingEvents = false;
            //        // Console.WriteLine("change");
            //        Console.WriteLine("文件{0}已经被修改,修改类型{1}", e.FullPath, e.ChangeType.ToString());
            //        ServiceController serv = new ServiceController("Autosolder");
            //        object s = serv.Status;
            //        // fun.closeThread();
            //        //fun = null;
            //        //fun = new AutosolderServiceFun();
            //        //string path = "D:\\AutosolderNet\\AutoSolderReStart.bat";
            //        // Console.WriteLine(path);
            //        //ExecuteBatFile(path);
            //        //RestartService();
            //        Environment.Exit(1);
            //        watcher.EnableRaisingEvents = true;
            //    };
            //    watcher.EnableRaisingEvents = true;
            //    Console.WriteLine("...");
            //    Console.ReadKey();
            //}
            //catch (Exception e)
            //{

            //    throw;
            //}
            //DbConfig dbconfig = new DbConfig();
            //dbconfig.Catalog = "dbtest";
            //dbconfig.Source = "localtest";
            //dbconfig.User = "roottest";
            //dbconfig.Password = "pempenn";
            //dbconfig.Port = "3306test";
            //Console.WriteLine(dbconfig.Catalog);
            //Console.WriteLine(dbconfig.Source);
            //Console.WriteLine(dbconfig.User);
            //Console.WriteLine(dbconfig.Password);
            //Console.WriteLine(dbconfig.Port);
            //Console.ReadKey();

        }

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
                    serv.Stop();
                    serv.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                }
                serv.Start();
                serv.WaitForStatus(ServiceControllerStatus.Running,timeout);

            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (serv.Status == ServiceControllerStatus.Stopped)
                {
                    serv.Start();
                    serv.WaitForStatus(ServiceControllerStatus.Running);
                }
                serv.Close();
            }
        }
    }
}
