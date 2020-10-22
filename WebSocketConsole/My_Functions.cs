using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using COMM;

namespace WebSocketConsole
{
    class My_Functions
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public static string encryptKey = "SalemSu!";



        public static string GetSetting_FromFile(String FilePath, ref String sErr)
        {
            sErr = "";
            string str = "";
            try
            {
                //获取文件内容
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.StreamReader file1 = new System.IO.StreamReader(FilePath);//读取文件中的数据
                    str = file1.ReadToEnd();                                            //读取文件中的全部数据
                    file1.Close();
                    file1.Dispose();
                }
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
            }
            return str;
        }

        public static String SaveSetting_ToFile(String sData, String sFileName, ref String sErr)
        {
            try
            {
                string CurDir = System.IO.Path.GetDirectoryName(sFileName);
                if (!System.IO.Directory.Exists(CurDir)) System.IO.Directory.CreateDirectory(CurDir);   //该路径不存在时，在当前文件目录下创建文件夹
                System.IO.StreamWriter file1 = new System.IO.StreamWriter(sFileName, false);     //文件已覆盖方式添加内容
                file1.Write(sData);                                                              //保存数据到文件
                file1.Close();                                                                  //关闭文件
                file1.Dispose();
                CurDir = "";
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
            }
            return "";
        }

        public static String GetIP()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "ipconfig.exe";//设置程序名     
            cmd.StartInfo.Arguments = "/all";  //参数     
                                               //重定向标准输出     
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;//不显示窗口（控制台程序是黑屏）     
                                                //cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//暂时不明白什么意思     
            /*  
            收集一下 有备无患  
            关于:ProcessWindowStyle.Hidden隐藏后如何再显示？  
            hwndWin32Host = Win32Native.FindWindow(null, win32Exinfo.windowsName);  
            Win32Native.ShowWindow(hwndWin32Host, 1);     //先FindWindow找到窗口后再ShowWindow  
            */
            cmd.Start();
            string info = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            cmd.Close();
            return info;
        }

        public static String GetIP_EX(ref String sErr)
        {
            String sIP = "";
            sErr = "";
            try
            {
                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                foreach (IPAddress ipa in ipadrlist)
                {
                    if (ipa.AddressFamily == AddressFamily.InterNetwork)
                        sIP += ipa.ToString();
                }
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
            }
            return sIP;
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串 </returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));//转换为字节
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();//实例化数据加密标准
                MemoryStream mStream = new MemoryStream();//实例化内存流
                //将数据流链接到加密转换的流
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        // <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        public string DataTableToJsonString(DataTable table, ref String sErr)
        {
            sErr = "";
            try
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(table);
                return JSONString;
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                return "";
            }
        }

        public DataTable JsonStringToDataTable(String JsonString, ref String sErr)
        {
            sErr = "";
            try
            {
                DataTable data = JsonConvert.DeserializeObject<DataTable>(JsonString);
                return data;
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                return null;
            }
        }
    }
}
