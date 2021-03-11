using demo.ClassFolder;
using demo.View.NomalView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zebra.Sdk.Comm;

namespace demo
{
    /// <summary>
    /// TextSearchPrints.xaml 的交互逻辑
    /// </summary>
    public partial class TextSearchPrints : Page
    {
        Printer printer;
        private string printerName = "";
        private AddPrinter add = null;
        private bool _isOpenPopup = false;
        private List<City> _printers;
        private List<City> _itemRefresh;
        private City _printer;
        private BackgroundWorker bgMeet;
        
        //private ObservableCollection<AddPrintersMessage> executePrinters = null;
        public TextSearchPrints(Window window)
        {
            InitializeComponent();
            //Printer.SetPrinters();
            _printers = Printer.SetPrinters(itemsPrinters);
            printersCount.Text =  Printer.printerCount.ToString();
            //txtPrintersState.Text = Printer.GetPrinterStatus()
            //AddExecutePrinters();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //读取打印机列表的第一项
            isVisibilityForEnter.Visibility = Visibility.Visible;
            var c = _printers.First();
            txtPrinterName2.Text = c.Name;
            txtPrinterNetStatus.Text = c.NetStatus.ToString();
            printerName = c.Name;
            _printer = c;
            if (c.NetStatus == 0)
            {
                btnDialogBox.IsEnabled = false;
            }
            /*else
            {
                try
                {
                    string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                }
                catch (Exception)
                {
                    Printer.ClosePrinter();
                }
            }*/

        }

        //展示ItemsControl里的打印机名
        private void printerMessage_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var c = btn.DataContext as City;
            txtPrinterName2.Text = c.Name;
            txtPrinterNetStatus.Text = c.NetStatus.ToString();
            printerName = c.Name;
            _printer = c;
            if (c.NetStatus == 0)
            {
                btnDialogBox.IsEnabled = false;
            }
            else
            {
                btnDialogBox.IsEnabled = true;
                try
                {
                    string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                }
                catch (Exception)
                {
                    Printer.ClosePrinter();
                }
            }
        }

        private void printerMessage_Initialized(object sender, EventArgs e)
        {

        }

        private void btnSentFile_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName);
            normalPrinter.btnSendFile.IsChecked = true;
            normalPrinter.ShowDialog();
        }

        private void btnHighSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName);
            normalPrinter.btnHighSetting.IsChecked = true;
            normalPrinter.ShowDialog();
        }

        private void btnPrintSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName);
            normalPrinter.btnPrintSetting.IsChecked = true;
            normalPrinter.ShowDialog();
            
            
        }

        private void btnBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName);
            normalPrinter.btnBasicMessage.IsChecked = true;
            normalPrinter.ShowDialog();
        }

        private void btnRafresh_Click(object sender, RoutedEventArgs e)
        {
            bgMeet = new BackgroundWorker();
            bgMeet.WorkerReportsProgress = true;
            bgMeet.DoWork += new DoWorkEventHandler(bgMeet_DoWork);
            bgMeet.ProgressChanged += new ProgressChangedEventHandler(bgMeet_ProgressChanged);
            bgMeet.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgMeet_RunWorkerCompleted);
            bgMeet.RunWorkerAsync();
            
        }

        private void btnAddPrinter_Click(object sender, RoutedEventArgs e)
        {
            add = new AddPrinter();
            add.ShowDialog();
        }

        private void btnDialogBox_Click(object sender, RoutedEventArgs e)
        {
            DeviceDetailsNomalView deviceDetails = new DeviceDetailsNomalView(_printer);
            skipPages.Content = deviceDetails;
        }

        private void btnMoreSetting_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
            //以下代码为，在popupOpen的时候再次点击按钮将IsOpen设为false，但有一些Bug，便不使用。
            /*if (_isOpenPopup == true)
            {
                _isOpenPopup = false;
                popup.IsOpen = false;
            }
                
            else
            {
                _isOpenPopup = true;
                popup.IsOpen = true;

            }*/

        }

        void bgMeet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            busyRefresh.IsActive = false;
            
            this.Dispatcher.Invoke(new Action(() =>
            {
                printersCount.Text = Printer.printerCount.ToString();
                itemsPrinters.ItemsSource = _itemRefresh;
                txtIsLoading.Text = "";
            }));
        }
        void bgMeet_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            /*this.Dispatcher.Invoke(new Action(() =>
            {
                this.txtIsLoading.Text = "多线程开启！";
            }));*/
        }



        void bgMeet_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                busyRefresh.IsActive = true;
                txtIsLoading.Text = "加载中";
            }));
            GetData();
        }
        public void GetData()
        {
            _itemRefresh = Printer.SetPrintersStatus();
            
        }
    }
}
