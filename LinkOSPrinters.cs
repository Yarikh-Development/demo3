using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            //try
            //{
                //monitorPrinter = new ObservableCollection<LinkOSPrinters>();
                /*RootObject rb = JsonConvert.DeserializeObject<RootObject>(jsonMessage);
                Console.WriteLine(rb.companyID);
                Console.WriteLine(rb.employees[0].firstName);
                foreach (Manager ep in rb.manager)
                {
                    Console.WriteLine(ep.age);
                }
                Console.ReadKey();*/
                String[] str = new string[3];
                JObject jobj = JObject.Parse(jsonMessage);
                JObject media = JObject.Parse(jobj["media"].ToString());
                JObject head = JObject.Parse(jobj["head"].ToString());
                JObject power = JObject.Parse(jobj["power"].ToString());

                str[0] = media["media.status"].ToString();
                str[1] = head["head.latch"].ToString();
                str[2] = power["power.percent_full"].ToString();
                /*monitorPrinter.Add(new LinkOSPrinters
                {
                    WareStstus = media["media.status"].ToString(),
                    HeadStatus = head["head.latch"].ToString()

                });*/
                return str;
                //Console.WriteLine("a:" + jobj["a"]);
                //Console.Read();
                //return null;
            //}
            //catch (Exception ex)
            //{
                //MessageBox.Show(ex.Message);
                //return null;
            //}
            
        }
    }
}
