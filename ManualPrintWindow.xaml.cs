using System;
using System.Windows;
using System.Windows.Controls;

namespace demo
{
	/// <summary>
	/// ManualPrintWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ManualPrintWindow : Window
	{
		private PicturesPage picturesPage;
		private TextMenuDetail textMenuDetail;
		private TextPicturesPage textPicturesPage;

		public ManualPrintWindow(Window window)
		{
			InitializeComponent();
			//设置父窗口
			this.Owner = window;

			//加载打印机列表
			Printer.SetPrinters(Printer_List_ComboBox);
		}

		public ManualPrintWindow(PicturesPage picturesPage)
		{
			this.picturesPage = picturesPage;
		}

		public ManualPrintWindow(TextMenuDetail textMenuDetail)
		{
			this.textMenuDetail = textMenuDetail;
		}

		public ManualPrintWindow(TextPicturesPage textPicturesPage)
		{
			InitializeComponent();
			
			Printer.SetPrinters(Printer_List_ComboBox);
			this.textPicturesPage = textPicturesPage;
		}

		//选择模板按钮
		private void ChoiceFileButtonClick(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			Template_TextBox.Text = FileTools.GetFileShortName(FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg", "*.png|*jpg"));
		}

		//打印按钮
		private void PrintButtonClick(object sender, RoutedEventArgs e)
		{
			if (Template_TextBox.Text == "")
            {
				textPicturesPage.PrintState("请选择模板！");
				return;
			}
			//发文件
			uint num = uint.Parse(Number_TextBox.Text);
			while (num-- > 0)
				Printer.SendFile(Printer_List_ComboBox.Text, FileTools.labelDirPath + "\\" + Template_TextBox.Text);
		}

		public string[] ShowDialogAndResult()
		{
			this.ShowDialog();
			string[] data = new string[] { DateTime.Now.ToShortTimeString().ToString(), Template_TextBox.Text, Printer_List_ComboBox.Text };
			foreach (string s in data)
			{
				if (s == "")
					return null;
			}
			return data;
		}
	}
}
