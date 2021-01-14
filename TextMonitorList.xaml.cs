using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace demo
{
    /// <summary>
    /// TextMonitorList.xaml 的交互逻辑
    /// </summary>
    public partial class TextMonitorList : Page
    {
        public static DispatcherTimer realTimeUpdata;
        public static ObservableCollection<LinkOSPrinters> linkOSPrinter2 = null;
        private WebSocketConsole.MainWindow mainWindow = null;
        public TextMonitorList( WebSocketConsole.MainWindow mainWindow)
        {
            InitializeComponent();
            //this.linkOSPrinter = linkOSPrinter;
            this.mainWindow = mainWindow;
            realTimeUpdata = new DispatcherTimer();
            realTimeUpdata.Tick += new EventHandler(RealTime);
            realTimeUpdata.Interval = new TimeSpan(0, 0, 5);
            //if (linkOSPrinter != null)
            //{
                realTimeUpdata.Start();
            //}
                
        }

        private void RealTime(object sender, EventArgs e)
        {
            if (TextHighPrinter.linkOSPrinter != null)
            {
                String command = "{}{\"media\":null,\"head\":null,\"power\":null,\"device\":null}";
                String command2 = "{}{\"media.status\":null,\"head.latch\":null,\"power.percent_full\":null,\"device.product_name\":null,\"odometer.media_marker_count1\":null}";
                String command3 = "{}{\"media.status\":null,\"head.latch\":null,\"power.percent_full\":null," +
                    "\"device.product_name\":null,\"odometer.media_marker_count1\":null,\"print.tone\":null," +
                    "\"media.speed\":null,\"media.printmode\":null}";
                String message = "";
                String[] str;
                ArrayList array;
                linkOSPrinter2 = TextHighPrinter.linkOSPrinter;
                foreach (var item in linkOSPrinter2)
                {
                    message = mainWindow.SendRawDataToPrinter(command3, item.SN);
                    //message有可能是：Device:XXZKJ181100121 is not online.需不需要做判断
                    //将Json数据保存到指定打印机的Json文件中
                    string file = $"{FileTools.LinkOSPrintersJsonPath}\\{item.SN}.json";
                    FileTools.WriteNewFile(file, message);
                    //将接收到的信息发送给JSON解析出想要的信息
                    str = LinkOSRealTime.SetJsonMessage(message); //返回一个数组
                    //array = LinkOSRealTime.SetJsonMessageForList(message);  //返回一个列表

                    if (str != null)
                    {
                        item.WareStatus = str[0];
                        item.HeadStatus = str[1];
                        item.PowerFull = str[2];
                        item.PrinterType = str[3];
                        item.PrintOdometer = str[4];
                    }
                    /*if (array != null)
                    {
                        item.WareStatus = array[0];
                        foreach (var msg in array)
                        {
                            item.WareStatus = msg.;
                        }
                    }*/

                }
                lvMonitor.ItemsSource = linkOSPrinter2;
            }
            else
            {
                //linkOSPrinter2.Clear();
                //lvMonitor.Items.Clear();
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTime();
            //realTimeUpdata.Stop();
        }

        public static void StopTime()
        {
            realTimeUpdata.Stop();
        }
    }
}
