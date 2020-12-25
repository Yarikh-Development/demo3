using BarcodeScanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RadioButton = System.Windows.Forms.RadioButton;
using demo.Quartz;

namespace demo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        private TextSearchPrints textSearchPrints = new TextSearchPrints();
        private TextFiles textFiles = null;
        private TextMenu textMenu = new TextMenu();
        private TextLogPage textLogPage = null;
        private TextHighPrinter TextHighPrinter = new TextHighPrinter();
		private RelationPage relationPage;
        public static bool isWriteLog;
        private ScanerHook listener = new ScanerHook();
        public Window1()
        {
            InitializeComponent();
            FileTools.Init();
        }

        //窗口缩放
        private void WindowSize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
                
            else
            {
                //窗口放大后不要挡住任务栏
                //this.MaxHeight = SystemParameters.WorkArea.Height;
                //this.MaxHeight = Screen.PrimaryScreen.Bounds.Height;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                WindowState = WindowState.Maximized;
                
            }
             
        }

        //关闭窗口
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //管理打印机
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = textSearchPrints;          
        }
        //扫描打印
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = textMenu;
        }

        //文件
		private void btnFiles_Click(object sender, RoutedEventArgs e)
		{
            textFiles = new TextFiles();
            frameShowPages.Content = textFiles;
		}

        //查看
		private void btnLogPage_Click(object sender, RoutedEventArgs e)
		{
            textLogPage = new TextLogPage();
            frameShowPages.Content = textLogPage;
		}

        //主界面？
		private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //最小化
        private void MiniWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //主界面？？
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = textMenu;
            Task.Run(() => CronTriggersQuartz.UsingQAsync());
            //System.Windows.Application.MainWindow = this;
            //this.MaxHeight = Screen.PrimaryScreen.Bounds.Height;
            //WindowState = WindowState.Maximized;
        }

        //创建用户设置界面
        private void CreateUserWindowClick(object sender, RoutedEventArgs e)
        {
            User userWindow = new User(this);
            userWindow.ShowDialog();
        }




        //创建打印机设置界面
        private void CreatePrinterWindowClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.PrintDialog p = new System.Windows.Forms.PrintDialog();
            p.ShowDialog();
        }

        //获取关联页面
        public RelationPage GetRelationPage()
        {
            return relationPage;
        }
        //获取记录页面
        public TextLogPage GetLogPage()
        {
			return textLogPage;
        }

		//获取关联记录
		private string[] GetRelationData(string number)
        {
            StreamReader streamReader = new StreamReader(FileTools.relationFilePath);
            string line = "";
            string[] data;

            while ((line = streamReader.ReadLine()) != null)
            {
                data = line.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 3 && number == data[0])
                    return data;
            }

            return null;
        }

		private void MainWindowDeactivated(object sender, EventArgs e)
        {
            this.listener.Stop();
        }

        private void MainWindowActivated(object sender, EventArgs e)
        {
            this.listener.Start();
        }

        private void TestWindow_Click(object sender, RoutedEventArgs e)
        {
            Window1 wd = new Window1();
            wd.Show();
        }

        private void LinkOSPrinters_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = TextHighPrinter;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnOpenPrinterConfigFile_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start(FileTools.logDirPath);
        }

        private void btnOpenLogPath_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(FileTools.logDirPath);
        }

        private void btnOpenLabelPath_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(FileTools.labelDirPath);
        }

        private void btnNormalPrinters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHighPrinters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClosingWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPrintLogs_Click(object sender, RoutedEventArgs e)
        {
            textLogPage = new TextLogPage();
            frameShowPages.Content = textLogPage;
            this.btnLogPage.IsChecked = true;
        }

        private void btnOrderList_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = TextHighPrinter;
            this.LinkOSPrinters.IsChecked = true;
            TextHighPrinter.btnOrderList.IsChecked = true;
            TextHighPrinter.OpenPageInitialize(4);
        }

        private void btnLabelList_Click(object sender, RoutedEventArgs e)
        {
            textFiles = new TextFiles();
            frameShowPages.Content = textFiles;
            btnFiles.IsChecked = true;
        }

        private void btnDeviceDetails_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = TextHighPrinter;
            this.LinkOSPrinters.IsChecked = true;
            TextHighPrinter.txtDialogBox.IsChecked = true;
            TextHighPrinter.OpenPageInitialize(3);
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBindingFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSendFeedBack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAboutApp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAboutYarikh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRealTimePage_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = TextHighPrinter;
            this.LinkOSPrinters.IsChecked = true;
            TextHighPrinter.btnHome.IsChecked = true;
            TextHighPrinter.OpenPageInitialize(1);
        }
    }
}
