using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Button = System.Windows.Controls.Button;

namespace demo
{

	public class CommandContent
    {
        public string Direct { get; set; }
        public string Explain { get; set; }
    }

    /// <summary>
    /// TextAdminInterface.xaml 的交互逻辑
    /// </summary>
    public partial class TextAdminInterface : Page
    {
		private ObservableCollection<CommandContent> commandContentSGD = null;
		private ObservableCollection<CommandContent> commandContentZPL = null;
		public TextAdminInterface()
        {
            InitializeComponent();

			//LoadData(FileTools.instructionsFilePath);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			try
			{
				commandContentZPL = new ObservableCollection<CommandContent>();
				string filePathZPL = FileTools.commandPath + "\\commandZPL.txt";
				FileStream fileStreamZPL = new FileStream(filePathZPL, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReaderZPL = new StreamReader(fileStreamZPL);

				commandContentSGD = new ObservableCollection<CommandContent>();
				string filePathSGD = FileTools.commandPath + "\\commandSGD.txt";
				FileStream fileStreamSGD = new FileStream(filePathSGD, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReaderSGD = new StreamReader(fileStreamSGD);

				string line;
				string[] data;
				while ((line = streamReaderSGD.ReadLine()) != null)
				{
					data = line.Split(';');
					commandContentSGD.Add(new CommandContent{ Direct = data[0], Explain = data[1] });
				}
				listView2.ItemsSource = commandContentSGD;

				while ((line = streamReaderZPL.ReadLine()) != null)
				{
					data = line.Split(';');
					commandContentZPL.Add(new CommandContent{ Direct = data[0], Explain = data[1] });
				}
				listView.ItemsSource = commandContentZPL;

				streamReaderZPL.Close();
				streamReaderSGD.Close();
			}
			catch (FileNotFoundException)
			{
				FileTools.WriteLineFile(FileTools.exceptionFilePath, "未找到指令文件！");
			}
			catch (DirectoryNotFoundException)
			{
				FileTools.WriteLineFile(FileTools.exceptionFilePath, " 指令文件路径为空！！");
			}
		}

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
			commandContentZPL.Clear();
			commandContentSGD.Clear();
		}

        private void btnSGDDelete_Click(object sender, RoutedEventArgs e)
        {
			var btn = sender as Button;

			var c = btn.DataContext as CommandContent;

			//MessageBox.Show(c.id);

			//移除当前点击按钮所在行
			commandContentSGD.Remove(c);

			//刷新item
			//list1.Items.Refresh();
		}
	}

}
