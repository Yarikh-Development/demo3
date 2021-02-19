using BarcodeScanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace demo
{
	/// <summary>
	/// TextPicturesPage.xaml 的交互逻辑
	/// </summary>
	public partial class TextPicturesPage : Page
    {
		//TextFiles textFiles = new TextFiles();
		TextLogPage textLogPage = new TextLogPage();

        public static bool isWriteLog;
        private ScanerHook listener = new ScanerHook();
		private RelationStruct[] texts = new RelationStruct[3];
		private string _openImagePath = "";
		//记录钩子事件传过来的值，其实就是扫描枪扫到的结果
        private string _pNumber = "";

        public string PNumber
        {
            get { return _pNumber; }
            private set { _pNumber = value; }
        }


        public TextPicturesPage()
        {
			InitializeComponent();
			//SetData();
			listener.ScanerEvent += ListenerScanerEvent;
		}

		//钩子事件处理
		private void ListenerScanerEvent(ScanerHook.ScanerCodes codes)
		{
			//设置能效编号
			if (Auto_MenuItem.IsChecked==true)
			{
				//将钩子捕获到的结果集展示在界面框里，但扫描时已经扫过一次在了，会导致显示两次信息。
				//0112，将自动打印的标签码输入框改成readonly就不会出现上面说的这个问题了。
				PNumber = codes.Result;	
				AutoPrint(codes.Result);
			}
		}

		//创建打印机设置界面
		private void CreatePrinterWindowClick(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.PrintDialog p = new System.Windows.Forms.PrintDialog();
			p.ShowDialog();
		}

		private void Auto_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			listener.Start();
			Manual_MenuItem.IsChecked = false;
			Auto_MenuItem.IsChecked = true;
			Start_Print_MenuItem.IsEnabled = false;
			//picturesPage.HideControl(true);
			this.HideControl(true);
			Refresh();
			
		}

		private void Manual_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			listener.Stop();
			Auto_MenuItem.IsChecked = false;
			Manual_MenuItem.IsChecked = true;
			Start_Print_MenuItem.IsEnabled = true;
			//picturesPage.HideControl(false);
			this.HideControl(false);
			Printer.SetPrinters(Printer_List_ComboBox);
			Refresh();
			
		}
		public void HideControl(bool state)
		{
			if (state) //自动
			{
				Overprint_Picture_Change_Button.IsEnabled = false;
				//panelManualPrint.Visibility = Visibility.Hidden;
			}
			else //手动
			{
				Overprint_Picture_Change_Button.IsEnabled = true;
				//panelManualPrint.Visibility = Visibility.Visible;
			}
		}
		private void Refresh()
        {
			EE_Number_TextBlock.Text = "";
			EE_Piceture_Image.Source = null;
			Overprint_Number_TextBlock.Text = "";
			Overprint_Picture_Image.Source = null;
			Preview_Number_TextBlock.Text = "";
			Preview_Picture_Image.Source = null;
			_openImagePath = "";
			txtAutoPrintNumber.Text = "";
			txtEE_Piceture.Visibility = Visibility.Visible;
			txtOverprint_Picture.Visibility = Visibility.Visible;
			txtPreview_Picture.Visibility = Visibility.Visible;
		}

		//自动打印
		private void AutoPrint(string number)
		{
			this.LoadPicture();
			//this.LoadPicture();
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
			if (textLogPage != null)
				textLogPage.WriteDataToListView(line);
			FileTools.WriteLineFile(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",
									line[0] + ';' + line[1] + ';' + line[2] + ';' + line[3] + ';');
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
			if (txtAutoPrintNumber.Text == "")
            {
				State_TextBox.Text = "请输入能效编号";
				return;
			}
			PNumber = txtAutoPrintNumber.Text;
			//输入编号，加载图片
			string result = this.LoadPicture();
			if (result != "")
			{
				if (result == "继续打印")
                {
					MessageBoxResult msgResult = MessageBox.Show("找不到模板，是否继续打印？", "", MessageBoxButton.YesNo);
					State_TextBox.Text = "找不到模板";
					if (msgResult == MessageBoxResult.No)
                    {
						return;
                    }
				}
                else
                {
					State_TextBox.Text = result;
					return;
				}
				
			}

			//打印机操作
			string[] data = new string[] { DateTime.Now.ToShortTimeString().ToString(), txtAutoPrintNumber.Text, Printer_List_ComboBox.Text };
			foreach (string s in data)
			{
				if (s == "")
					data =  null;
			}

			//发文件
			uint num = uint.Parse(txtPrintCount.Text);
			while (num-- > 0)
            {
				bool isPrint;
				//记录是否打印，其实是记录是否异常，打印机不在线或者其他未发现的情况不视为异常，有待解决
				if (_openImagePath == "")
                {
					isPrint = Printer.SendFile(Printer_List_ComboBox.Text, FileTools.FindFiles(FileTools.labelDirPath, EE_Number_TextBlock.Text, ".png"));
				}
				else
                {
					isPrint = Printer.SendFile(Printer_List_ComboBox.Text, _openImagePath);
				}
					

				//保存记录到日志文件
				//时间戳，模板编码，手动输入的编码（亦是模板编码亦是能效编码，应该是能效编码，有待测试），打印机名称
				if (isPrint)
                {
					if (textLogPage != null)
						textLogPage.WriteDataToListView(data);
					FileTools.WriteLineFile($"{FileTools.logDirPath}\\{DateTime.Now:yyyy-MM-dd}.txt",
						$"{data[0]};{PNumber};{data[1]};{data[2]};");
					isWriteLog = true;
				}
				
			}				
		}

		//消息显示
		public void PrintState(string msg)
		{
			State_TextBox.Text = msg;
		}

        private void PictureChangeButtonClick(object sender, RoutedEventArgs e)
        {
			Button button = sender as Button;
			//string path = FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg|*.jepg|*.jepg", ".png");
			string path = FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg", "*.png|*jpg");
			_openImagePath = path;
			if (path.Length > 4)
            {
				txtAutoPrintNumber.Text = FileTools.GetFileShortName(path).Split('.')[0];
				BitmapImage imagesouce = new BitmapImage(new Uri(path));
				Overprint_Picture_Image.Source = imagesouce.Clone();
			}
        }

        

        public string LoadPicture()
        {
            if (PNumber == "")
            {
				return "没有编码";
            }
            BitmapImage[] imagesouce = new BitmapImage[3];
            string EEPath = FileTools.FindFiles(FileTools.pictureDirPath, PNumber, ".jpg");
            string overprintPath = FileTools.FindFiles(FileTools.labelDirPath, PNumber, ".png");
            string previewPath = FileTools.FindFiles(FileTools.pictureDirPath, PNumber, ".png");

            if (overprintPath != "")
            {
                Overprint_Picture_Image.Source = new BitmapImage(new Uri(overprintPath)).Clone();
                Overprint_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath).Split('.')[0];
				txtOverprint_Picture.Visibility = Visibility.Hidden;
            }
            else
            {
				if (_openImagePath != "")                
					return "继续打印";
                else 
					return "找不到模板";
            }

			if (EEPath != "")
			{
				EE_Piceture_Image.Source = new BitmapImage(new Uri(EEPath)).Clone();
				EE_Number_TextBlock.Text = FileTools.GetFileShortName(EEPath).Split('.')[0];
				txtEE_Piceture.Visibility = Visibility.Hidden;
			}

			if (previewPath != "")
            {
                Preview_Picture_Image.Source = new BitmapImage(new Uri(previewPath)).Clone();
                Preview_Number_TextBlock.Text = FileTools.GetFileShortName(previewPath).Split('.')[0];
				txtPreview_Picture.Visibility = Visibility.Hidden;
            }
            return "";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
			listener.Stop();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			listener.Start();
		}

        
    }
}
