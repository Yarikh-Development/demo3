using BarcodeScanner;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace demo
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private RelationPage relationPage;
		private PicturesPage picturesPage;
		private LogPage logPage;
		public static bool isWriteLog;
		private ScanerHook listener = new ScanerHook();

		public MainWindow()
		{
			InitializeComponent();
			FileTools.Init();
			TabItem tabitem = new TabItem();
			tabitem.Header = "图片";
			Frame tabFrame = new Frame();
			picturesPage = new PicturesPage();
			tabFrame.Content = picturesPage;
			tabitem.Content = tabFrame;
			Displaying_TabControl.Items.Add(tabitem);

			listener.ScanerEvent += ListenerScanerEvent;
			/*if (IsOnlyOneProcess())
			{
				InitializeComponent();
				FileTools.Init();
				TabItem tabitem = new TabItem();
				tabitem.Header = "图片";
				Frame tabFrame = new Frame();
				picturesPage = new PicturesPage();
				tabFrame.Content = picturesPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);

				listener.ScanerEvent += ListenerScanerEvent;
			}
			else
				this.Close();*/
		}

		//进程判断
		private bool IsOnlyOneProcess()
		{
			int cnt = 0;
			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName.ToLower() == "demo")
					++cnt;
			}

			if (cnt == 1)
				return true;
			else
				return false;
		}

		//钩子事件处理
		private void ListenerScanerEvent(ScanerHook.ScanerCodes codes)
		{
			//设置能效编号
			if (Auto_MenuItem.IsChecked)
            {
				picturesPage.SetNumber(codes.Result);
				AutoPrint(codes.Result);
			}
		}

		//创建打印机设置界面
		private void CreatePrinterWindowClick(object sender, RoutedEventArgs e)
		{
            System.Windows.Forms.PrintDialog p = new System.Windows.Forms.PrintDialog();
			p.ShowDialog();
		}

		
		//创建用户设置界面
		private void CreateUserWindowClick(object sender, RoutedEventArgs e)
		{
			User userWindow = new User(this);
			userWindow.ShowDialog();
		}

		//创建DataList界面
		private void CreateDataListWindowClick(object sender, RoutedEventArgs e)
		{
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem.Name == Log_MenuItem.Name && logPage == null)
			{
				TabItem tabitem = new TabItem();
				tabitem.Header = "记录";
				Frame tabFrame = new Frame();
				logPage = new LogPage(this);
				tabFrame.Content = logPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);
				if (relationPage == null)
					Displaying_TabControl.SelectedIndex = 1;
				else
					Displaying_TabControl.SelectedIndex = 2;
			}
			else if (menuItem.Name == Relation_MenuItem.Name && relationPage == null)
			{
				TabItem tabitem = new TabItem();
				tabitem.Header = "关联";
				Frame tabFrame = new Frame();
				relationPage = new RelationPage(this);
				tabFrame.Content = relationPage;
				tabitem.Content = tabFrame;
				Displaying_TabControl.Items.Add(tabitem);
				if (logPage == null)
					Displaying_TabControl.SelectedIndex = 1;
				else
					Displaying_TabControl.SelectedIndex = 2;
			}
		}

		//获取关联页面
		public RelationPage GetRelationPage()
        {
			return relationPage;
        }

		//获取记录页面
		public LogPage GetLogPage()
		{
			return logPage;
		}

		//标题按钮设置
		private void MinWindowButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaxWindowButtonClick(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
		}

		private void ExitWindowButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void WindowTitleMouseLeftButtonDown(object sender, RoutedEventArgs e)
		{
			DragMove();
		}

		/*
		 * 
		 */

		//打印模式选择
		private void PrintModeClick(object sender, RoutedEventArgs e)
        {
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem.Name == "Auto_MenuItem")
            {
				listener.Start();
				Manual_MenuItem.IsChecked = false;
				Auto_MenuItem.IsChecked = true;
				Start_Print_MenuItem.IsEnabled = false;
				picturesPage.HideControl(true);
			}
			else if (menuItem.Name == "Manual_MenuItem")
            {
				listener.Stop();
				Auto_MenuItem.IsChecked = false;
				Manual_MenuItem.IsChecked = true;
				Start_Print_MenuItem.IsEnabled = true;
				picturesPage.HideControl(false);
			}

			picturesPage.EE_Number_TextBlock.Text = "";
			picturesPage.EE_Piceture_Image.Source = null;
			picturesPage.Overprint_Number_TextBlock.Text = "";
			picturesPage.Overprint_Picture_Image.Source = null;
			picturesPage.Preview_Number_TextBlock.Text = "";
			picturesPage.Preview_Picture_Image.Source = null;
        }

		//自动打印
		private void AutoPrint(string number)
        {
			picturesPage.LoadPicture();
			string[] data = GetRelationData((string)number);
			if (data == null)
				return;
			//发送文件								
			bool result = Printer.SendFile(data[2], FileTools.labelDirPath + "\\" + data[1] + ".png");
			if (!result)
			{
				State_TextBox.Text = DateTime.Now.ToShortTimeString().ToString() + "发送文件失败！";
				return;
			}

			string[] line = { DateTime.Now.ToShortTimeString().ToString(), data[0], data[1], data[2] };
			//记录写入
			if (logPage != null)
				logPage.WriteDataToListView(line);
			FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
									line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';');
			isWriteLog = true;
		}

		//手动打印
		private void ManualPrint()
		{
			//输入编号，加载图片
			string result = picturesPage.LoadPicture();
			if (result != "")
			{
				State_TextBox.Text = DateTime.Now.ToShortTimeString().ToString() + result;
				return;
			}
			//打印机操作
			ManualPrintWindow manualPrintWindow = new ManualPrintWindow(this);
			string[] data = manualPrintWindow.ShowDialogAndResult();
			
			if (data.Length != 4)
				return;

			string[] line = { data[0], picturesPage.GetNumber(), data[1], data[2], data[3] };
			if (logPage != null)
				logPage.WriteDataToListView(line);
			FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
							line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';' + line[4] + ';');
			isWriteLog = true;
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

		//开始手动打印
		private void PrintClick(object sender, RoutedEventArgs e)
		{
			ManualPrint();
		}

		/*
		 * 
		 */

		//消息显示
		public void PrintState(string msg)
        {
			State_TextBox.Text = msg;
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
    }
}
