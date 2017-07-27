using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    public class DbConfig
    {
        public DbConfig()
        {

        }



        private static Configuration _config = null;

        private static void openConfig()
        {
            if (_config == null)
            {
                //try
                //{
                string sPath = "D:\\AutosolderNet\\Preferences.xml";
                //Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                //sPath = Path.Combine(Path.Combine(sPath, "DBconfig"), "Preferences.xml");
                try
                {
                    if (!File.Exists(sPath))
                    {

                        if (!Directory.Exists(Path.GetDirectoryName(sPath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(sPath));
                        }
                        // CreateConfig(sPath);
                        string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<configuration>\r\n</configuration>";
                        using (StreamWriter sw = new StreamWriter(sPath, false, Encoding.UTF8))
                        {
                            sw.Write(xml);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClass.WriteLogFile("Class-DbConfig;Fun-openConfig; 43行:" + ex.Message);
                    //throw;
                }
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = sPath;
                _config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                
            }
        }

   

        public static void writeConfig(string sKey, string sValue)
        {
            openConfig();

            if (_config.AppSettings.Settings[sKey] == null)
                _config.AppSettings.Settings.Add(sKey, sValue);
            else
                _config.AppSettings.Settings[sKey].Value = sValue;

            //_config.Save(ConfigurationSaveMode.Modified);
            _config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings"); //刷新，否则程序读取的还是之前的值（可能已装入内存）
            _config = null;
        }

        public static string readConfig(string sKey, string sDefValue)
        {
            openConfig();

            if (_config.AppSettings.Settings[sKey] == null)
            {
                _config.AppSettings.Settings.Add(sKey, sDefValue);
                _config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings"); //刷新，否则程序读取的还是之前的值（可能已装入内存）
            }

            return _config.AppSettings.Settings[sKey].Value;
        }
        /// <summary>
        /// 数据库服务器的DNS名称或IP
        /// </summary>
        public string Source
        {
            get { return readConfig("DBServer", "localhost"); }
            set { writeConfig("DBServer", value); }
        }
        /// <summary>
        /// 默认的数据连接端口
        /// </summary>
        public string Port
        {
            get { return readConfig("DBPort", "3306"); }
            set { writeConfig("DBPort", value); }
        }
        /// <summary>
        /// 默认的数据库
        /// </summary>
        public string Catalog
        {
            get { return readConfig("DBCatalog", "autosolder"); }
            set { writeConfig("DBCatalog", value); }
        }
        /// <summary>
        /// 连接数据库的用户名
        /// </summary>
        public string User
        {
            get { return readConfig("DBUser", "root"); }
            set { writeConfig("DBUser", value); }
        }
        /// <summary>
        /// 用于更改数据库连接的密码
        /// </summary>
        public string Password { set { writeConfig("DBPwd", EncryptDES(value, rgbIV, rgbKey)); } get { return DecryptDES(readConfig("DBPwd", "n8Cd3eQq3bg="), rgbIV, rgbKey); } }
        

        private static readonly byte[] rgbIV = { 182, 186, 218, 219, 190, 182, 186, 188 };
        private static readonly byte[] rgbKey = { 223, 226, 225, 132, 146, 147, 206, 213 };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, byte[] rgbIV, byte[] rgbKey)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, byte[] rgbIV, byte[] rgbKey)
        {
            if (string.IsNullOrEmpty(decryptString))
                return "";

            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

    }
}
