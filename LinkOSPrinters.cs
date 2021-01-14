using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace demo
{
    public class LinkOSPrinters
    {
        public int ID { get; set; }
        public string ClientID { get; set; }    //不明所以的ID，不知道是什么
        public string PrinterName { get; set; } //打印机名，用户自己命名
        public string DeviceName { get; set; }  //打印机连接名
        public string SN { get; set; }
        public string FWVersion { get; set; }
        public string IP { get; set; }
        public string GatewayIP { get; set; }   //网关，一直是空的，估计也用不上
        public string  Type { get; set; }       //暂时不知道有什么用
        public string WareStatus { get; set; }  //这个应该是切纸状态
        public string HeadStatus { get; set; }  //打印头状态
        public string PaperStatus { get; set; } //耗材-切纸状态
        public string RibbonStatus { get; set; }//耗材-碳带状态
        public string PowerFull { get; set; }   //电量百分比
        public string PrinterType { get; set; } //型号
        public string PrintOdometer { get; set; }  //打印里程
        public string Speed { get; set; }       //速度
        public string Tone { get; set; }        //浓度
        public string PrintMode { get; set; }   //操作模式
        public string PaperMode { get; set; }   //纸张类型
        public string TrackMode { get; set; }   //跟踪模式

        //读取打印里程日志，返回一个集合
        public static List<LinkOSPrinters> CalcOdometer(DateTime logFileName)
        {
            try
            {
                var printOdometer = new List<LinkOSPrinters>();
                string filePathOdometer = FileTools.odometerLogPath + "\\" + logFileName.ToString("yyyy-MM-dd") + ".txt";
                FileStream fileStreamOdometer = new FileStream(filePathOdometer, FileMode.Open, FileAccess.Read);
                StreamReader streamReaderOdometer = new StreamReader(fileStreamOdometer);
                //streamReaderOdometer.ReadLine[0]
                string line;
                string[] data;
                while ((line = streamReaderOdometer.ReadLine()) != null)
                {
                    data = line.Split(';');
                    printOdometer.Add(new LinkOSPrinters { SN = data[1], IP = data[2], PrintOdometer = data[3] });
                }                
                streamReaderOdometer.Close();
                
                return Calc(printOdometer);
            }
            catch (FileNotFoundException)
            {
                string msg = $"没有关于{logFileName:yyyy-MM-dd}的打印数据！";
                MessageBox.Show(msg);
                FileTools.WriteLineFile(FileTools.exceptionFilePath, $"{DateTime.Now} {msg}");
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " 日志文件路径为空！！");
                return null;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
            catch(NotSupportedException)
            {
                MessageBox.Show("不支持给定数据的格式！");
                FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " 不支持给定数据的格式！！！");
                return null;
            }
        }

        //读取打印里程日段日志，返回一个集合
        public static List<LinkOSPrinters> CalcOdometer(DateTime logFileName1,DateTime logFileName2)
        {
            try
            {
                DateTime logFileName = logFileName1;
                var printOdometer = new List<LinkOSPrinters>();
                while (logFileName1 <= logFileName2)
                {
                    string filePathOdometer = FileTools.odometerLogPath + "\\" + logFileName1.ToString("yyyy-MM-dd") + ".txt";
                    if (!File.Exists(filePathOdometer))
                    {
                        logFileName1 = logFileName1.AddDays(1);
                        continue;
                    }
                        
                    FileStream fileStreamOdometer = new FileStream(filePathOdometer, FileMode.Open, FileAccess.Read);
                    StreamReader streamReaderOdometer = new StreamReader(fileStreamOdometer);
                    
                    string line;
                    string[] data;
                    while ((line = streamReaderOdometer.ReadLine()) != null)
                    {
                        data = line.Split(';');
                        printOdometer.Add(new LinkOSPrinters { SN = data[1], IP = data[2], PrintOdometer = data[3] });
                    }                    
                    streamReaderOdometer.Close();
                    logFileName1 = logFileName1.AddDays(1);
                }
                if (printOdometer.Count == 0)
                {
                    MessageBox.Show($"没有关于{logFileName:yyyy-MM-dd}至{logFileName2:yyyy-MM-dd}的打印数据！");
                    return null;
                }                    
                return Calc(printOdometer);
            }
            catch (DirectoryNotFoundException)
            {
                FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " 日志文件路径为空！！");
                return null;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("不支持给定数据的格式！");
                FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " 不支持给定数据的格式！！！");
                return null;
            }
        }

        //计算日段内的里程，返回计算好里程的集合
        public static List<LinkOSPrinters> Calc(List<LinkOSPrinters> listOdometer)
        {
            
            ArrayList array = new ArrayList();
            var list = new List<LinkOSPrinters>();
            array.Add(listOdometer.First().SN);
            list.Add(listOdometer.First());
            foreach (var item in listOdometer)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (!array.Contains(item.SN))
                    {
                        array.Add(item.SN);
                        list.Add(item);
                    }        
                }
            }
            var odo = new List<LinkOSPrinters>();
            foreach (var item in list)
            {
                var CalcOdo1 = listOdometer.Where(info => info.SN == item.SN).ToList();
                int num = Int32.Parse(CalcOdo1.Last().PrintOdometer) - Int32.Parse(CalcOdo1.First().PrintOdometer);
                odo.Add(new LinkOSPrinters { IP = item.IP ,SN = item.SN,PrintOdometer = num.ToString()});
            }
            return odo;          
        }
    }

    /// <summary>
    /// 定时检测打印机，并将打印机的信息返回
    /// </summary>
    public class LinkOSRealTime
    {
        private static ObservableCollection<LinkOSPrinters> monitorPrinter = null;
        public LinkOSRealTime()
        {

        }

        public static String[] SetJsonMessage(String jsonMessage)
        {
            try
            {
                //monitorPrinter = new ObservableCollection<LinkOSPrinters>();
                /*RootObject rb = JsonConvert.DeserializeObject<RootObject>(jsonMessage);
                Console.WriteLine(rb.companyID);
                Console.WriteLine(rb.employees[0].firstName);
                foreach (Manager ep in rb.manager)
                {
                    Console.WriteLine(ep.age);
                }
                Console.ReadKey();*/
                
                String[] str = new string[5];
                JObject jobj = JObject.Parse(jsonMessage);
                /*JObject media = JObject.Parse(jobj["media"].ToString());
                JObject head = JObject.Parse(jobj["head"].ToString());
                JObject power = JObject.Parse(jobj["power"].ToString());
                JObject device = JObject.Parse(jobj["device"].ToString());*/
                //String printOdometer = (jobj["odometer.media_marker_count1"].ToString().Split(',')[1]).Split(' ')[0];

                str[0] = jobj["media.status"].ToString();
                str[1] = jobj["head.latch"].ToString();
                str[2] = (jobj["power.percent_full"] ?? "").ToString();
                str[3] = (jobj["device.product_name"] ?? "").ToString();
                str[4] = (jobj["odometer.media_marker_count1"] ?? "").ToString();
                return str;
                //Console.WriteLine("a:" + jobj["a"]);
                //Console.Read();
                //return null;
            }
            catch (Exception ex)
            {
                //当接收到的数据为空时会抛出异常，返回空值
                //MessageBox.Show(ex.Message);
                return null;
            }
            
        }

        public static ArrayList JsonMessageForList(String jsonMessage)
        {
            try
            {
                ArrayList arrayList = new ArrayList();
                JObject jobj = JObject.Parse(jsonMessage);
                arrayList.Add(jobj["media.status"].ToString());
                arrayList.Add(jobj["head.latch"].ToString());
                arrayList.Add((jobj["power.percent_full"] ?? "").ToString());
                arrayList.Add((jobj["device.product_name"] ?? "").ToString());
                arrayList.Add((jobj["odometer.media_marker_count1"] ?? "").ToString());
                return arrayList;
            }
            catch (Exception ex)
            {
                //当接收到的数据为空时会抛出异常，返回空值
                //MessageBox.Show(ex.Message);
                return null;
            }

        }
    }
}
