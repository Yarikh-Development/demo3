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
using MessageBox = System.Windows.MessageBox;

namespace demo
{

	public class CommandContent
    {
        public string Direct { get; set; }
        public string Explain { get; set; }
        public string DetailExplain { get; set; }
    }

    /// <summary>
    /// TextAdminInterface.xaml 的交互逻辑
    /// </summary>
    public partial class TextAdminInterface : Page
    {
		private ObservableCollection<CommandContent> commandContentSGD = null;
		private ObservableCollection<CommandContent> commandContentZPL = null;
		private TextCommandSetting commandSetting = null;
		private int countZPL = 0;
		private int countSGD = 0;
		private string filePathZPL = "";
		private string filePathSGD = "";
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
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " 路径为空！！");
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
				filePathZPL = FileTools.commandPath + "\\commandZPL.txt";
				FileStream fileStreamZPL = new FileStream(filePathZPL, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReaderZPL = new StreamReader(fileStreamZPL);

				commandContentSGD = new ObservableCollection<CommandContent>();
				filePathSGD = FileTools.commandPath + "\\commandSGD.txt";
				FileStream fileStreamSGD = new FileStream(filePathSGD, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReaderSGD = new StreamReader(fileStreamSGD);

				string line;
				string[] data;
				while ((line = streamReaderSGD.ReadLine()) != null)
				{
					data = line.Split(';');
					commandContentSGD.Add(new CommandContent{ Direct = data[0], Explain = data[1], DetailExplain = data[2] });
				}
				countSGD = commandContentSGD.Count;
				listView2.ItemsSource = commandContentSGD;

				while ((line = streamReaderZPL.ReadLine()) != null)
				{
					data = line.Split(';');
					commandContentZPL.Add(new CommandContent{ Direct = data[0], Explain = data[1], DetailExplain = data[2] });
				}
				countZPL = commandContentZPL.Count;
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
			catch(IndexOutOfRangeException)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, "指令配置文件出错，请前往bin/Command/目录下查看源文件");
			}
		}

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
			if (countSGD != commandContentSGD.Count || countZPL != commandContentZPL.Count)
            {
				MessageBoxResult result = MessageBox.Show("尚有未保存的更改，是否保存更改！","", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					btnSave_Click(sender, e);
					
				}
				
				
            }
			commandContentZPL.Clear();
			commandContentSGD.Clear();
		}

        private void btnSGDDelete_Click(object sender, RoutedEventArgs e)
        {
			var btn = sender as Button;
			var c = btn.DataContext as CommandContent;
			//移除当前点击按钮所在行
			commandContentSGD.Remove(c);

			//刷新item
			//list1.Items.Refresh();
		}

		private void btnZPLDelete_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var c = btn.DataContext as CommandContent;
			commandContentZPL.Remove(c);
		}

		private void btnAddCommand_Click(object sender, RoutedEventArgs e)
        {
			commandSetting = new TextCommandSetting();
			commandSetting.btnEdit.IsChecked = true;
			commandSetting.ShowDialog();		
		}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
			List<string> strList = new List<string>();
			try
            {
				if (countSGD == commandContentSGD.Count && countZPL == commandContentZPL.Count)
				{
					MessageBox.Show("没有删除操作，无需保存！");
					return;
				}
				if (countZPL != commandContentZPL.Count)
                {
					foreach (var item in commandContentZPL)
					{
						strList.Add($"{item.Direct};{item.Explain};{item.DetailExplain};");
					}
					File.WriteAllLines(filePathZPL, strList.ToArray());
					strList.Clear();
				}
				
				if (countSGD != commandContentSGD.Count)
                {
					foreach (var item in commandContentSGD)
					{
						strList.Add($"{item.Direct};{item.Explain};{item.DetailExplain};");
					}
					File.WriteAllLines(filePathSGD, strList.ToArray());
					strList.Clear();
				}
				
					
				MessageBox.Show("保存成功");
			}
            catch (Exception ex)
            {

				MessageBox.Show(ex.Message);
            }
            finally
            {
				strList.Clear();
			}
		}

        private void btnEditCommand_Click(object sender, RoutedEventArgs e)
        {
			
			var btn = sender as Button;
			var c = btn.DataContext as CommandContent;
			listView.SelectedItem = c;
			commandSetting = new TextCommandSetting(c.Direct,c.Explain,"",listView.SelectedIndex,"ZPL");
			commandSetting.btnEdit.IsChecked = true;
			commandSetting.ShowDialog();			
		}

        private void btnEditSGDCommand_Click(object sender, RoutedEventArgs e)
        {
			var btn = sender as Button;
			var c = btn.DataContext as CommandContent;
			listView2.SelectedItem = c;
			commandSetting = new TextCommandSetting(c.Direct, c.Explain, c.DetailExplain, listView2.SelectedIndex, "SGD");
			commandSetting.btnEdit.IsChecked = true;
			commandSetting.ShowDialog();
		}

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
			Page_Loaded(sender,e);
        }
    }
}
