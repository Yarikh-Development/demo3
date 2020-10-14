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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextMenuDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TextMenuDetail : Page
    {

		private RelationPage relationPage;
		private PicturesPage picturesPage;
		private TextLogPage textLogPage;
		public static bool isWriteLog;
		private ScanerHook listener = new ScanerHook();

		private RelationStruct[] texts = new RelationStruct[3];



		public TextMenuDetail()
        {
            InitializeComponent();
			SetData();
			ClearNumber();

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
		/*private bool IsOnlyOneProcess()
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
		}*/

		//钩子事件处理
		private void ListenerScanerEvent(ScanerHook.ScanerCodes codes)
		{
			//设置能效编号
			if (Auto_MenuItem.IsChecked)
			{
				//picturesPage.SetNumber(codes.Result);
				this.SetNumber(codes.Result);
				AutoPrint(codes.Result);
			}
		}

		//获取关联页面
		/*public RelationPage GetRelationPage()
		{
			return relationPage;
		}

		//获取记录页面
		public TextLogPage GetLogPage()
		{
			return textLogPage;
		}*/


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
				//picturesPage.HideControl(true);
				this.HideControl(true);
			}
			else if (menuItem.Name == "Manual_MenuItem")
			{
				listener.Stop();
				Auto_MenuItem.IsChecked = false;
				Manual_MenuItem.IsChecked = true;
				Start_Print_MenuItem.IsEnabled = true;
				//picturesPage.HideControl(false);
				this.HideControl(false);
			}

			/*picturesPage.EE_Number_TextBlock.Text = "";
			picturesPage.EE_Piceture_Image.Source = null;
			picturesPage.Overprint_Number_TextBlock.Text = "";
			picturesPage.Overprint_Picture_Image.Source = null;
			picturesPage.Preview_Number_TextBlock.Text = "";
			picturesPage.Preview_Picture_Image.Source = null;*/

			//this.EE_Number_TextBlock.Text = "";
			//this.EE_Piceture_Image.Source = null;
			//this.Overprint_Number_TextBlock.Text = "";
			//this.Overprint_Picture_Image.Source = null;
			//this.Preview_Number_TextBlock.Text = "";
			//this.Preview_Picture_Image.Source = null;

			this.EE_Number_TextBlock.Text = "";
			this.EE_Piceture_Image.Source = null;
			this.EE_Number_TextBlock.Text = "";
			this.EE_Piceture_Image.Source = null;
			this.EE_Number_TextBlock.Text = "";
			this.EE_Piceture_Image.Source = null;

		}

		//自动打印
		private void AutoPrint(string number)
		{
			//picturesPage.LoadPicture();
			this.LoadPicture();
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

		//手动打印
		private void ManualPrint()
		{
			//输入编号，加载图片
			//string result = picturesPage.LoadPicture();
			string result = this.LoadPicture();
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

			//string[] line = { data[0], picturesPage.GetNumber(), data[1], data[2], data[3] };
			string[] line = { data[0], this.GetNumber(), data[1], data[2], data[3] };
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

		/*private void MainWindowDeactivated(object sender, EventArgs e)
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
		}*/




		public void SetNumber(string name)
		{
			EE_Number_TextBlock.Text = name;
		}

		//获取编号
		public string GetNumber()
		{
			return EE_Number_TextBlock.Text;
		}

		//设置编号
		private void SetData()
		{
			texts[0].obj = new List<FrameworkElement>();
			texts[1].obj = new List<FrameworkElement>();
			texts[2].obj = new List<FrameworkElement>();
			//设置字符串对应的TextBox控件
			//texts[0].name = "EE_Picture_Change_Button";
			//texts[0].obj.Add(EE_Number_TextBlock);
			//texts[0].obj.Add(EE_Piceture_Image);
			//texts[1].name = "Overprint_Picture_Change_Button";
			//texts[1].obj.Add(Overprint_Number_TextBlock);
			//texts[1].obj.Add(Overprint_Picture_Image);
			//texts[2].name = "Preview_Picture_Change_Button";
			//texts[2].obj.Add(Preview_Number_TextBlock);
			//texts[2].obj.Add(Preview_Picture_Image);

			texts[0].name = "EE_Picture_Change_Button";
			texts[0].obj.Add(EE_Number_TextBlock);
			texts[0].obj.Add(EE_Piceture_Image);
			texts[1].name = "Overprint_Picture_Change_Button";
			texts[1].obj.Add(EE_Number_TextBlock);
			texts[1].obj.Add(EE_Piceture_Image);
			texts[2].name = "Preview_Picture_Change_Button";
			texts[2].obj.Add(EE_Number_TextBlock);
			texts[2].obj.Add(EE_Piceture_Image);

		}

		private void PictureChangeButtonClick(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			string path = FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg|*.jepg|*.jepg", ".png");

			if (path.Length > 4)
			{
				string name = FileTools.GetFileShortName(path);
				foreach (RelationStruct keyValue in texts)
				{
					if (keyValue.name == button.Name)
					{
						BitmapImage imagesouce = new BitmapImage();
						imagesouce = new BitmapImage(new Uri(path));
						((Image)keyValue.obj[1]).Source = imagesouce.Clone();
						((TextBox)keyValue.obj[0]).Text = name.Split(new char[] { '.' })[0];
						break;
					}
				}
			}
		}

		public void HideControl(bool state)
		{
			if (state) //自动
			{
				EE_Picture_Change_Button.IsEnabled = false;
				Overprint_Picture_Change_Button.IsEnabled = false;
				Preview_Picture_Change_Button.IsEnabled = false;
				EE_Number_TextBlock.IsReadOnly = false;
			}
			else //手动
			{
				EE_Picture_Change_Button.IsEnabled = true;
				Overprint_Picture_Change_Button.IsEnabled = true;
				Preview_Picture_Change_Button.IsEnabled = true;
				EE_Number_TextBlock.IsReadOnly = false;
			}
		}

		public string LoadPicture()
		{
			string number = EE_Number_TextBlock.Text;
			if (number == "")
				return "请输入能效编号";
			BitmapImage[] imagesouce = new BitmapImage[3];
			string EEPath = FileTools.FindFiles(FileTools.pictureDirPath, EE_Number_TextBlock.Text, ".jpg");
			string overprintPath = FileTools.FindFiles(FileTools.labelDirPath, EE_Number_TextBlock.Text, ".png");
			string previewPath = FileTools.FindFiles(FileTools.pictureDirPath, EE_Number_TextBlock.Text, ".png");

			if (EEPath != "")
			{
				EE_Piceture_Image.Source = new BitmapImage(new Uri(EEPath)).Clone();
			}

			if (overprintPath != "")
			{

				EE_Piceture_Image.Source = new BitmapImage(new Uri(overprintPath)).Clone();
				//Overprint_Picture_Image.Source = new BitmapImage(new Uri(overprintPath)).Clone();
				//Overprint_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
				EE_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
			}

			if (previewPath != "")
			{
				EE_Piceture_Image.Source = new BitmapImage(new Uri(previewPath)).Clone();
				//Preview_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
				EE_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
			}
			return "";
		}

		public void ClearNumber()
		{
			//MainWindow mainWindow = new MainWindow();
			//if (MainWindow.isWriteLog)
			EE_Number_TextBlock.Text = "";
			MainWindow.isWriteLog = false;
		}


	}
}
