using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace demo
{
	/// <summary>
	/// RelationPage.xaml 的交互逻辑
	/// </summary>
	public partial class RelationPage : Page
	{
		private Window window = null;
		private TextPicturesPage page;

		public RelationPage(Window window)
		{
			InitializeComponent();

			this.window = window;

			LoadData();
			Relation_ListView.SelectedIndex = 0;
		}

		//加载关联文件
		public void LoadData()
		{
			try
			{
				StreamReader streamReader = new StreamReader(FileTools.relationFilePath);

				string line;
				string[] data;
				while ((line = streamReader.ReadLine()) != null)
				{
					if (line != "")
					{
						data = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						Relation_ListView.Items.Add(new { number = data[0], template = data[1], printer = data[2] });
					}
				}

				streamReader.Close();
				
			}
			/*catch (FileNotFoundException)
			{
				((MainWindow)window).PrintState("文件未找到！！");
			}
			catch (ArgumentNullException)
			{
				((MainWindow)window).PrintState("路径为空！！");
			}*/
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
			//创建编辑窗口
			RelationEditWindow relationEditWindow = new RelationEditWindow(Relation_ListView.SelectedIndex);
			relationEditWindow.ShowDialog();
			//更新数据
			List<string> lines = new List<string>(File.ReadAllLines(FileTools.relationFilePath));
			string[] data = lines[Relation_ListView.SelectedIndex].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (data.Length == 3)
				Relation_ListView.Items[Relation_ListView.SelectedIndex] = new { number = data[0], template = data[1], printer = data[2] };
		}

		//添加记录
		private void AddRelationRecord(object sender, RoutedEventArgs e)
		{
			//创建编辑窗口
			RelationEditWindow relationEditWindow = new RelationEditWindow();
			bool? flag = relationEditWindow.ShowDialog();
			//获取新数据
			if (flag == true)
            {
				List<string> lines = new List<string>(File.ReadAllLines(FileTools.relationFilePath));
				string[] data = lines[lines.Count - 1].Split(new char[] { ';' });
				Relation_ListView.Items.Add(new { number = data[0], template = data[1], printer = data[2] });
			}
		}

		//删除记录
		private void DeleteRelationRecord(object sender, RoutedEventArgs e)
		{
			if (IsClickLine())
			{
				FileTools.DeleteLine(FileTools.relationFilePath, Relation_ListView.SelectedIndex);
				Relation_ListView.Items.RemoveAt(Relation_ListView.SelectedIndex);
			}
		}

		//判断点击是否在行
		private bool IsClickLine()
		{
			return Relation_ListView.SelectedIndex != -1;
		}
	}

}
