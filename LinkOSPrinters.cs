using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
    class LinkOSPrinters
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

    }
}
