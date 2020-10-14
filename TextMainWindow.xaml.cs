using BarcodeScanner;
using System;
using System.Collections.Generic;
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

namespace demo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        //private TextMenu textMenu;
        //private TextFile textFile;
        //private TextSearchPrints textSearchPrints;
        TextSearchPrints textSearchPrints = new TextSearchPrints();
        TextFiles textFiles = new TextFiles();
        TextMenu textMenu = new TextMenu();
        TextLogPage textLogPage = new TextLogPage();
        //private TextLogPage textLogPage;
		private RelationPage relationPage;
        private TextPicturesPage textPicturesPage;

        //private RadioButton radioButton;


        public static bool isWriteLog;
        private ScanerHook listener = new ScanerHook();


		//string State_TextBoxs = TextPicturesPage.State_TextBox;


        public Window1()
        {
            //textPicturesPage = new TextPicturesPage();
            //this.State_TextBoxs.Text = TextPicturesPage.State_TextBox;
            //InitializeComponent();
            if (IsOnlyOneProcess())
            {

                

                InitializeComponent();

                //this.State_TextBoxs.Text = TextPicturesPage.State_TextBox;

                FileTools.Init();
                TabItem tabitem = new TabItem();
                tabitem.Header = "图片";
                Frame tabFrame = new Frame();
                textPicturesPage = new TextPicturesPage();
                tabFrame.Content = textPicturesPage;
                tabitem.Content = tabFrame;
                //Displaying_TabControl.Items.Add(tabitem);
                



                listener.ScanerEvent += ListenerScanerEvent;
            }
            else
                this.Close();

        }

        //进程判断
		private bool IsOnlyOneProcess()
		{

            

            //throw new NotImplementedException();
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
			/*if (Auto_MenuItem.IsChecked)
			{
				//picturesPage.SetNumber(codes.Result);
                textPicturesPage.SetNumber(codes.Result);
                AutoPrint(codes.Result);
			}*/
		}




		public void SkipPages()
        {

           
        }

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

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            frameShowPages.Content = textSearchPrints;          
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = textMenu;
        }

		private void btnFiles_Click(object sender, RoutedEventArgs e)
		{
			frameShowPages.Content = textFiles;
		}

		private void btnLogPage_Click(object sender, RoutedEventArgs e)
		{
			frameShowPages.Content = textLogPage;
		}

		private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MiniWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frameShowPages.Content = textMenu;
            this.MaxHeight = Screen.PrimaryScreen.Bounds.Height;
            WindowState = WindowState.Maximized;
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

        //创建DataList界面
        /*private void CreateDataListWindowClick(object sender, RoutedEventArgs e)
        {
            //System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            *//*if (menuItem.Name == Log_MenuItem.Name && textLogPage == null)
            {
                TabItem tabitem = new TabItem();
                tabitem.Header = "记录";
                Frame tabFrame = new Frame();
                textLogPage = new TextLogPage(this);
                tabFrame.Content = textLogPage;
                tabitem.Content = tabFrame;
                Displaying_TabControl.Items.Add(tabitem);
                if (relationPage == null)
                    Displaying_TabControl.SelectedIndex = 1;
                else
                    Displaying_TabControl.SelectedIndex = 2;
                
            }
            btnFiles
            else if (menuItem.Name == Relation_MenuItem.Name && relationPage == null)
            {
                TabItem tabitem = new TabItem();
                tabitem.Header = "关联";
                Frame tabFrame = new Frame();
                relationPage = new RelationPage(this);
                tabFrame.Content = relationPage;
                tabitem.Content = tabFrame;
                Displaying_TabControl.Items.Add(tabitem);
                if (textLogPage == null)
                    Displaying_TabControl.SelectedIndex = 1;
                else
                    Displaying_TabControl.SelectedIndex = 2;
            }*//*
            //System.Windows.Controls.RadioButton radioButton = sender as System.Windows.Controls.RadioButton;
            if (radioButton.Name == btnLogPage.Name && textLogPage == null)
            {
                TabItem tabitem = new TabItem();
                //tabitem.Header = "记录";
                Frame tabFrame = new Frame();
                textLogPage = new TextLogPage(this);
                tabFrame.Content = textLogPage;
                tabitem.Content = tabFrame;
               *//* Displaying_TabControl.Items.Add(tabitem);
                if (relationPage == null)
                    Displaying_TabControl.SelectedIndex = 1;
                else
                    Displaying_TabControl.SelectedIndex = 2;*//*

            }
            
            else if (radioButton.Name == btnFiles.Name && relationPage == null)
            {
                TabItem tabitem = new TabItem();
                //tabitem.Header = "关联";
                Frame tabFrame = new Frame();
                relationPage = new RelationPage(this);
                tabFrame.Content = relationPage;
                tabitem.Content = tabFrame;
                *//*Displaying_TabControl.Items.Add(tabitem);
                if (textLogPage == null)
                    Displaying_TabControl.SelectedIndex = 1;
                else
                    Displaying_TabControl.SelectedIndex = 2;*//*
            }
        }*/




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


        //打印模式选择
        private void PrintModeClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem.Name == "Auto_MenuItem")
            {
                listener.Start();
                /*Manual_MenuItem.IsChecked = false;
                Auto_MenuItem.IsChecked = true;*/
                //Start_Print_MenuItem.IsEnabled = false;
                //picturesPage.HideControl(true);
                textPicturesPage.HideControl(true);
            }
            else if (menuItem.Name == "Manual_MenuItem")
            {
                listener.Stop();
                /*Auto_MenuItem.IsChecked = false;
                Manual_MenuItem.IsChecked = true;*/
                //Start_Print_MenuItem.IsEnabled = true;
                //picturesPage.HideControl(false);
                textPicturesPage.HideControl(false);
            }

            /*picturesPage.EE_Number_TextBlock.Text = "";
            picturesPage.EE_Piceture_Image.Source = null;
            picturesPage.Overprint_Number_TextBlock.Text = "";
            picturesPage.Overprint_Picture_Image.Source = null;
            picturesPage.Preview_Number_TextBlock.Text = "";
            picturesPage.Preview_Picture_Image.Source = null;*/

            textPicturesPage.EE_Number_TextBlock.Text = "";
            textPicturesPage.EE_Piceture_Image.Source = null;
            textPicturesPage.Overprint_Number_TextBlock.Text = "";
            textPicturesPage.Overprint_Picture_Image.Source = null;
            textPicturesPage.Preview_Number_TextBlock.Text = "";
            textPicturesPage.Preview_Picture_Image.Source = null;
        }

        //自动打印
        private void AutoPrint(string number)
        {
            textPicturesPage.LoadPicture();
            string[] data = GetRelationData((string)number);
            if (data == null)
                return;
			//发送文件								
			bool result = Printer.SendFile(data[2], FileTools.labelDirPath + "\\" + data[1] + ".png");
			if (!result)
			{
				//State_TextBox.Text = DateTime.Now.ToShortTimeString().ToString() + "发送文件失败！";
				return;
			}

			string[] line = { DateTime.Now.ToShortTimeString().ToString(), data[0], data[1], data[2] };
            //记录写入
            if (textLogPage != null)
                textLogPage.WriteDataToListView(line);
            FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
                                    line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';');
            isWriteLog = true;
        }

		//手动打印
		private void ManualPrint()
		{
			//输入编号，加载图片
			string result = textPicturesPage.LoadPicture();
			if (result != "")
			{
				//State_TextBox.Text = DateTime.Now.ToShortTimeString().ToString() + result;
				return;
			}
			//打印机操作
			ManualPrintWindow manualPrintWindow = new ManualPrintWindow(this);
			string[] data = manualPrintWindow.ShowDialogAndResult();

			if (data.Length != 4)
				return;

			string[] line = { data[0], textPicturesPage.GetNumber(), data[1], data[2], data[3] };
			if (textLogPage != null)
				textLogPage.WriteDataToListView(line);
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

		//消息显示
		/*public void PrintState(string msg)
		{
			State_TextBox.Text = msg;
		}*/

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
