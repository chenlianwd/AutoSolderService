using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoSolder.BLL
{
    public  class WindowsService
    {


        public static string path = String.Empty;

        public static string GetPath(string serviceName)
        {
            if (ServiceIsExisted(serviceName) && path != "")
            {
                string s = "\"";
                s = GetString(path,s.ToCharArray());
                return s;
            }
            else
            {
                return "";
            }
        }
        private static bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    if (FilePath(s.ServiceName) != "")
                    {
                        path = FilePath(s.ServiceName);
                    }
                    return true;
                }
            }
            return false;
        }
        public static string FilePath(string serviceName)
        {


            RegistryKey _Key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Services\" + serviceName);
            if (_Key != null)
            {
                object _ObjPath = _Key.GetValue("ImagePath");
                if (_ObjPath != null) return _ObjPath.ToString();
            }
            return "";

        }
            ///<summary>

        /// 截前后字符串

        ///</summary>

         ///<param name="val">原字符串</param>

         ///<param name="str">要截掉的字符串</param>

        ///<param name="bAllStr">是否对整个字符串进行截取

        ///如果为true则对整个字符串中匹配的进行截取

        ///如果为false则只截取前缀或者后缀</param>

        ///<returns></returns>

        private static string GetString(string val, string str, bool bAllStr)
        {

          return Regex.Replace(val, @"(^(" + str + ")" + (bAllStr? "*" : "") + "|(" + str + ")" + (bAllStr? "*" : "") + "$)", "");

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="val">原字符串</param>
        /// <param name="c">要截取的字符</param>
        /// <returns></returns>
        private static string GetString(string val, char[] c)
        {
            return val.TrimStart(c).TrimEnd(c);
        }


    }
}
