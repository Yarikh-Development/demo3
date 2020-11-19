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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextAdminInterface.xaml 的交互逻辑
    /// </summary>
    public partial class TextAdminInterface : Page
    {
        public TextAdminInterface()
        {
            InitializeComponent();

			LoadData(FileTools.instructionsFilePath);
			SetComboBox();

		}

		public void SetComboBox()
		{
			try
			{
				string[] directorieStrings = Directory.GetFiles(FileTools.logDirPath);

				int cnt = 0;
				foreach (string s in directorieStrings)
				{
					//Log_File_ComboBox.Items.Add(s);
					++cnt;
				}
				//Log_File_ComboBox.SelectedIndex = cnt - 1;
			}
			catch (ArgumentNullException)
			{
				FileTools.WriteLineFile(DateTime.Now.ToString() + FileTools.exceptionFilePath, " 路径为空！！");
			}
		}

		private void ZPL_Click(object sender, RoutedEventArgs e)
		{
			this.ZPL.Visibility = Visibility.Visible;
			this.SGD.Visibility = Visibility.Hidden;

			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				using (StreamReader sr = new StreamReader(ofd.FileName))
				{
					string s = sr.ReadToEnd();
					string[] lines = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var i in lines)
					{
						string[] data = i.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
						listView.Items.Add(new { Direct = data[0], Explain = data[1] });
					}
				}
			}
		}
		private void SGD_Click(object sender, RoutedEventArgs e)
		{
			this.SGD.Visibility = Visibility.Visible;
			this.ZPL.Visibility = Visibility.Hidden;
		}

		private void Preview_Click(object sender, RoutedEventArgs e)
		{

		}
		private void Copy_Click(object sender, RoutedEventArgs e)
		{

		}
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			/*//删除选中的某项
			var btn = sender as Button;
			var c = btn.DataContext as demo.CShowTag;
			int index = int.Parse(c.Id);
			this.listView.Items.GetItemAt(index);
			MessageBoxResult boxResult = MessageBox.Show($"确定删除：id={c.Id},Qr={c.Qr} 吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (boxResult == MessageBoxResult.Yes)
			{
				_bindingClass.ShowList.Remove(c);
				this.listView.Items.Refresh();
			}
			return;*/
		}


		//加载记录文件
		public void LoadData(string filePath)
		{
			//DateTime.Now.ToShortTimeString().ToString();
			//DateTime.Now.ToString();
			//DateTime.Now.ToShortTimeString().ToString();

			try
			{
				FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReader = new StreamReader(fileStream);

				string line;
				string[] data;
				while ((line = streamReader.ReadLine()) != null)
				{
					data = line.Split(new char[] { ';' });
					/*Log_ListView.Items.Add(new { date = data[0], number = data[1], template = data[2], printer = data[3], success = data[4] });*/
					listView.Items.Add(new { Direct = data[0], Explain = data[1],success = data[2] });
				}

				streamReader.Close();
			}
			catch (FileNotFoundException)
			{
				FileTools.WriteLineFile(DateTime.Now.ToString() + " " + FileTools.exceptionFilePath, "未找到文件！");
			}
		}

		//写记录到listView
		public void WriteDataToListView(string[] data)
		{
			//string date = Log_File_ComboBox.SelectedItem.ToString();
			//string date =null;
			//string todayFile = FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			if (data.Length == 2)
			{
				/*Log_ListView.Items.Add(new { date = data[0], number = data[1], template = data[2], printer = data[3] });*/
				listView.Items.Add(new { Direct = data[0], Explain = data[1] });
			}
		}



	}

}
