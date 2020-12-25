using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.Quartz
{
    class GetLinkOSPrintersToQuartz : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => GetPrinters());
        }
        private void GetPrinters()
        {
            ObservableCollection<LinkOSPrinters> printers = TextMonitorList.linkOSPrinter2;
            string data = "";
            string path = FileTools.odometerLogPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if (printers != null)
            {
                foreach (var item in printers)
                {
                    data = DateTime.Now.ToString() + $";{item.SN};{item.IP};{item.PrintOdometer};";
                    FileTools.WriteLineFile(path, data);
                }
                
            }
            
        }
    }
}
