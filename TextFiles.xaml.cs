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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextFiles.xaml 的交互逻辑
    /// </summary>
    public partial class TextFiles : Page
    {

        //private Window window = null;
		private TextPicturesPage page = new TextPicturesPage();

		ObservableCollection<PrintersTemplate> list;

		//public TextFiles(Window window)
		public TextFiles()
		{
            InitializeComponent();
			
			//this.window = window;

			LoadData();
            Relation_ListView.SelectedIndex = 0;


        }

		//加载关联文件
		public void LoadData()
		{
			try
			{
				StreamReader streamReader = new StreamReader(FileTools.relationFilePath);
				list = new ObservableCollection<PrintersTemplate>(); ;
				string line;
				string[] data;
				while ((line = streamReader.ReadLine()) != null)
				{
					if (line != "")
					{
						data = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						list.Add(new PrintersTemplate { number = data[0], template = data[1], printer = data[2] });
					}
				}
				Relation_ListView.ItemsSource = list;
				streamReader.Close();

			}
			//catch (FileNotFoundException)
			//{
			//	((MainWindow)window).PrintState("文件未找到！！");
			//}
			//catch (ArgumentNullException)
			//{
			//	((MainWindow)window).PrintState("路径为空！！");
			//}
			catch (FileNotFoundException)
			{
				((TextPicturesPage)page).PrintState("文件未找到！！");
			}
			catch (ArgumentNullException)
			{
				((TextPicturesPage)page).PrintState("路径为空！！");
			}
		}


		//编辑记录
		private void ChangeRelationRecord(object sender, RoutedEventArgs e)
		{
            try
            {
				RelationEditWindow relationEditWindow = new RelationEditWindow(Relation_ListView.SelectedIndex);
				relationEditWindow.ShowDialog();
				//更新数据
				List<string> lines = new List<string>(File.ReadAllLines(FileTools.relationFilePath));
				//string[] data = lines[Relation_ListView.SelectedIndex].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				//if (data.Length == 3)
				//	Relation_ListView.Items[Relation_ListView.SelectedIndex] = new { number = data[0], template = data[1], printer = data[2] };
				LoadData();
			}
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                
            }
			//创建编辑窗口
			
		}

		//添加记录
		private void AddRelationRecord(object sender, RoutedEventArgs e)
		{
            try
            {
				//创建编辑窗口
				RelationEditWindow relationEditWindow = new RelationEditWindow();
				bool? flag = relationEditWindow.ShowDialog();
				//获取新数据
				if (flag == true)
				{
					List<string> lines = new List<string>(File.ReadAllLines(FileTools.relationFilePath));
					//string[] data = lines[lines.Count - 1].Split(new char[] { ';' });
					//Relation_ListView.Items.Add(new { number = data[0], template = data[1], printer = data[2] });
					LoadData();
				}
			}
            catch (Exception ex)
            {

				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
			}
			
		}

		//删除记录
		private void DeleteRelationRecord(object sender, RoutedEventArgs e)
		{
            try
            {
				if (IsClickLine())
				{
					FileTools.DeleteLine(FileTools.relationFilePath, Relation_ListView.SelectedIndex);
					//Relation_ListView.Items.RemoveAt(Relation_ListView.SelectedIndex);
					LoadData();
				}
			}
            catch (Exception ex)
            {

				MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
			
		}

		//刷新页面
		private void RefreshRelationRecord(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("刷新页面");
			//new TextFiles();
			//Relation_ListView.Items.Refresh();
			LoadData();
		}

		//判断点击是否在行
		private bool IsClickLine()
		{
			return Relation_ListView.SelectedIndex != -1;
		}

        
    }

	public class PrintersTemplate
    {
        public string number { get; set; }
        public string template { get; set; }
        public string printer { get; set; }
    }
}
