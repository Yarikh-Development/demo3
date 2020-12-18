using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using WebSocketConsole;
using WebSockets;

namespace demo
{
    /// <summary>
    /// TextHighPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class TextHighPrinter : Page
    {
        //WebServer server = null;
        Printer printer = null;
        String sTemp = "";
        public static ObservableCollection<LinkOSPrinters> linkOSPrinter = null;
        DeviceDetails deviceDetails;
        TextBasicSituation basicSituation;
        TextAdminInterface adminInterface;
        private DispatcherTimer dispatcherTimer;
        //private DispatcherTimer realTimeUpdata;
        WebSocketConsole.MainWindow mainWindow = new WebSocketConsole.MainWindow();
        //private Timer timerClose;
        public TextHighPrinter()
        {
            
            

            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Timer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //timerClose = new Timer(new TimerCallback(RealTime2), this, 5000, 0);
            /*realTimeUpdata = new DispatcherTimer();
            realTimeUpdata.Tick += new EventHandler(RealTime);
            realTimeUpdata.Interval = new TimeSpan(0, 0, 5);*/

            btnStopService.IsEnabled = false;
        }

        private void txtDialogBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                //skipPages.Content = deviceDetails;
                deviceDetails = new DeviceDetails(mainWindow, txtPrinterSN.Text, FileTools.commandPath);
                //deviceDetails.ShowDialog();
                skipPages.Content = deviceDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title,MessageBoxButton.OK);                
            }
            
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrmSettings frm = new FrmSettings();
                frm.Title = "Settings";
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void btnStartService_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.StartService(btnStartService, btnStopService);
            
            dispatcherTimer.Start();
            //realTimeUpdata.Start();
            btnDMP.IsEnabled = true;
        }

        

        private void btnStopService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainWindow.StopService();                
                linkOSPrinter.Clear();
                printersCount.Text = "0";
                dispatcherTimer.Stop();
                //realTimeUpdata.Stop();
                TextMonitorList.realTimeUpdata.Stop();
                btnStartService.IsEnabled = true;
                btnStartService.Focus();
                btnStopService.IsEnabled = false;
                btnDMP.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                btnStartService.IsEnabled = true;
                btnStopService.IsEnabled = false;
                
            }
            
        }

        private void Timer(object sender, EventArgs e)
        {
            try
            {
                int linkosPrinterNum = 0;
                String sTempLine = "";
                linkOSPrinter = new ObservableCollection<LinkOSPrinters>();
                sTemp = mainWindow.sTemp;
                if (sTemp != "" && sTemp != "0|")
                {
                    linkosPrinterNum++;
                    //MessageBox.Show(mainWindow.sTemp);
                    printersCount.Text = sTemp.Split('|')[0];
                    int nTotal = System.Text.RegularExpressions.Regex.Matches(sTemp, Environment.NewLine).Count;
                    for (int i = 0; i < nTotal; i++)
                    {
                        sTempLine = sTemp.Split('\n')[i];
                        if (i == 0)
                        {
                            linkOSPrinter.Add(new LinkOSPrinters
                            {
                                ID = linkosPrinterNum,
                                ClientID = sTempLine.Split('|')[1],
                                DeviceName = sTempLine.Split('|')[2],
                                FWVersion = sTempLine.Split('|')[3],
                                SN = sTempLine.Split('|')[4],
                                IP = sTempLine.Split('|')[5],
                                Type = sTempLine.Split('|')[8]
                            });
                        }
                        else if (i > 0)
                        {
                            linkOSPrinter.Add(new LinkOSPrinters
                            {
                                ID = linkosPrinterNum,
                                ClientID = sTempLine.Split('|')[0],
                                DeviceName = sTempLine.Split('|')[1],
                                FWVersion = sTempLine.Split('|')[2],
                                SN = sTempLine.Split('|')[3],
                                IP = sTempLine.Split('|')[4],
                                Type = sTempLine.Split('|')[7]
                            });
                        }
                    }
                }
                itemsPrinters.ItemsSource = linkOSPrinter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                
            }
            
        }

        //这里写个方法每个30秒发送LinkOS打印机的集合到某个类里获取到想要检测的信息，然后展示
        /*private void RealTime(object sender, EventArgs e)
        {
            if (linkOSPrinter != null)
            {

                String command = "{}{\"media\":null,\"head\":null}";                    
                String message = "";
                String[] str;
                foreach (var item in linkOSPrinter)
                {
                    message = mainWindow.SendRawDataToPrinter(command,item.SN.Substring(1));
                    //将接收到的信息发送给JSON解析出想要的信息
                    str = LinkOSRealTime.SetJsonMessage(message);
                    item.WareStatus = str[0];
                    item.HeadStatus = str[1];
                }
                lvMonitor.ItemsSource = linkOSPrinter;
                //return linkOSPrinter;
                //DispatcherTimer realTimeUpdata = new DispatcherTimer();
            }
            else
            {
                
            }
        }*/

        

        private void btnDMP_Click(object sender, RoutedEventArgs e)
        {
            new AnalysisDMP(mainWindow).Show() ;
        }

        private void btnPrinterDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                basicSituation = new TextBasicSituation();
                skipPages.Content = basicSituation;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);

            }
        }

        private void btnOrderList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                adminInterface = new TextAdminInterface();
                skipPages.Content = adminInterface;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        //点击按钮找到打印机列表的SN码
        private void printerMessage_Click_1(object sender, RoutedEventArgs e)
        {
            printer = new Printer();
            //isVisibilityForEnter.Visibility = Visibility.Visible;
            btnPrinterDetail.IsChecked = true;
            basicSituation = new TextBasicSituation();
            skipPages.Content = basicSituation;
            List<Button> buttonItems = printer.GetChildObjects<Button>(itemsPrinters, "");
            foreach (Button button in buttonItems)
            {
                if (button.IsFocused)
                {
                    TextBlock txt = printer.FindFirstVisualChild<TextBlock>(button, "txtPrintersSN");
                    txtPrinterSN.Text = txt.Text.Substring(1);
                }
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            TextMonitorList textMonitorList = new TextMonitorList(mainWindow);
            skipPages.Content = textMonitorList;
        }

        //private void 

        /*public void StopService(Button btnStart, Button btnStop)
        {
            try
            {
                String sErr = "";
                
                // timer1.Enabled = false;
                server.Dispose();
                server = null;
                serviceFactory = null;
                _logger = null;

                My_CommObject.Close(ref sErr);
                My_CommObject = null;

                btnStart.IsEnabled = true;
                btnStart.Focus();
                btnStop.IsEnabled = false;
                

                
                picOrange.Visibility = Visibility.Visible;
                picGreen.Visibility = Visibility.Hidden;
                // pnlSC.BackColor = Color.PaleGreen;
                
                
                
                msCurrentSelectedDeviceSN = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;               
                picOrange.Visibility = Visibility.Visible;
                picGreen.Visibility = Visibility.Hidden;               
            }
        }*/
    }
}
