using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoSolder.BLL
{
    public class AutoSolderBatMain
    {
        public AutoSolderBatMain(string batDirectory)
        {

        }
        private static void CreateAutoSolderInstallBatFile(string batpath)
        {
            string str = "";

        }
        private static void startServiceBatFile(string batPath)
        {

        }
        public static bool ExecuteBat(string batPath)
        {
            bool result = false;
            try
            {
                using (Process pro = new Process())
                {
                   
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.CreateNoWindow = true;
                   // pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                    pro.StartInfo.FileName = batPath;
                   
                    pro.StartInfo.Verb = "runas";
                    pro.Start();
                    pro.WaitForExit();
                }

                result = true;
            }
            catch (Exception e)
            {
                //写入日志
                //System.Diagnostics.Trace.WriteLine(e.Message);
                LogClass.WriteLogFile("Class-AutoSolderBatMain;Fun-ExecuteBat;行号50:" + e.Message);
            }
            return result;
        }

        private static void CreateBatFile(string context, string path)
        {

            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine(context);
            }
            catch(Exception ex)
            {
                LogClass.WriteLogFile("Class-AutoSolder;Fun-CreateBatFile();行号73：" + ex.Message);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
                if (fs != null)
                    fs.Close();
            }
        }
    }  
}
