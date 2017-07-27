using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using Common;

namespace AutoSolderService
{
    public class INI : ILDO
    {
        public string LDOPath
        { get;set; }

        public void WriteFun(string line, string ip, string port)
        {
            if (LDOPath == string.Empty || LDOPath == null)
            {
                return;

            }
            getLDOFile();
            Thread writeINI_Thread = new Thread(() => 
            {
                writeINI_File(line, ip, port);
            });
            writeINI_Thread.Start();
        }

       
        public List<NetConfig> ReadFun()
        {
            List<NetConfig> netList = new List<NetConfig>();
            try
            {
               
                if (LDOPath == string.Empty || LDOPath == null)
                {
                    return null;

                }

                if (!File.Exists(LDOPath))
                {
                    // File.Create(LDOPath);
                    if (!Directory.Exists(Path.GetDirectoryName(LDOPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LDOPath));
                    }
                    FileStream fs = new FileStream(LDOPath, FileMode.Create);
                    fs.Close();
                }
                string[] section = new string[100];
                if (INIHelper.GetAllSectionNames(out section, LDOPath) == -1)
                {
                    return null;
                }

                for (int i = 0; i < section.Length; i++)
                {
                    NetConfig netC = new NetConfig();
                    netC.Ip = INIHelper.Read(section[i], "ip", LDOPath);
                    netC.Port = INIHelper.Read(section[i], "port", LDOPath);
                    netC.Line = INIHelper.Read(section[i], "line", LDOPath);
                    netList.Add(netC);
                }
            }
            catch (Exception ex)
            {
                LogClass.WriteLogFile("Class-INI;Fun-ReadFun; 71行" + ex.Message);
                return null;
            }
            
            return netList;
            
        }

        
        private void getLDOFile()
        {
            try
            {
                if (!File.Exists(LDOPath))
                {

                    if (!Directory.Exists(Path.GetDirectoryName(LDOPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LDOPath));
                    }
                    FileStream fs = new FileStream(LDOPath, FileMode.Create);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                LogClass.WriteLogFile("Class-INI;Fun-getLDOFile; 82行:" + ex.Message);
                //throw;
            }
           


        }



        private void writeINI_File(string line, string ip, string port)
        {
            INIHelper.Write(line, "ip", ip, LDOPath);
            INIHelper.Write(line, "port", port, LDOPath);
            INIHelper.Write(line, "line", line, LDOPath);
        }

       


       

       

        
    }
    public class NetConfig
    {
        public NetConfig()
        {
            Ip = "192.168.0.0";
            Port = "1234";
            Line = "";
        }
        private string ip;
        private string port;
        private string line;

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public string Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public string Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
    }
}
