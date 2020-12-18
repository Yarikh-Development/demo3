using System;
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
        private ObservableCollection<LinkOSPrinters> linkOSPrinter2 = null;
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
                String command = "{}{\"media\":null,\"head\":null,\"power\":null}";
                String message = "";
                String[] str;
                linkOSPrinter2 = TextHighPrinter.linkOSPrinter;
                foreach (var item in linkOSPrinter2)
                {
                    message = mainWindow.SendRawDataToPrinter(command, item.SN.Substring(1));
                    //将接收到的信息发送给JSON解析出想要的信息
                    str = LinkOSRealTime.SetJsonMessage(message);
                    if (str != null)
                    {
                        item.WareStatus = str[0];
                        item.HeadStatus = str[1];
                        item.PowerFull = str[2];
                    }

                }
                lvMonitor.ItemsSource = linkOSPrinter2;
            }
            else
            {
                linkOSPrinter2.Clear();
                //lvMonitor.Items.Clear();
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            realTimeUpdata.Stop();
        }
    }
}
