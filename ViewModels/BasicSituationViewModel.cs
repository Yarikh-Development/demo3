using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace demo.ViewModels
{
    class BasicSituationViewModel : BindableBase
    {
        private LinkOSPrinters _pBasic;

        public LinkOSPrinters PBasicMsg
        {
            get { return _pBasic; }
            set
            {
                _pBasic = value;
                this.RaisePropertyChanged("PBasicMsg");
            }
        }

        public BasicSituationViewModel(string SN)
        {
            PBasicMsg = GetBasicMsg(SN);
        }

        //根据SN从Json文件中获取打印机的基本信息
        private LinkOSPrinters GetBasicMsg(string SN)
        {

            try
            {
                LinkOSPrinters printer;
                string jsonFilePath = $"{FileTools.LinkOSPrintersJsonPath}\\{SN}.json";
                String jsonMsg = FileTools.ReaderAll(jsonFilePath);
                if (jsonMsg != "")
                {
                    JObject jobj = JObject.Parse(jsonMsg);
                    printer = new LinkOSPrinters()
                    {
                        HeadStatus = (jobj["head.latch"] ?? "").ToString(),
                        PowerFull = (jobj["power.percent_full"] ?? "").ToString(),
                        PrinterType = (jobj["device.product_name"] ?? "").ToString(),
                        PrintOdometer = (jobj["odometer.media_marker_count1"] ?? "").ToString(),
                        Tone = (jobj["print.tone"] ?? "0").ToString(),
                        Speed = (jobj["media.speed"] ?? "0").ToString(),
                        PrintMode = (jobj["media.printmode"] ?? "").ToString()
                    };

                    return printer;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
