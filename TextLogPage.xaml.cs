﻿using System;
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
	/// TextLogPage.xaml 的交互逻辑
	/// </summary>
	public partial class TextLogPage : Page
	{
		//private Window window = null;
		private Window1 window1;

		//public TextLogPage(Window window)
		public TextLogPage()
		{
			InitializeComponent();

			
			//LoadData(FileTools.logDirPath+"\\"+DateTime.Now.ToString("yyyy-MM-dd")+".txt");
			SetComboBox();
		}

		public TextLogPage(Window1 window1)
		{
			this.window1 = window1;
		}

		~TextLogPage()
		{
			//todayFileStream.Close();
		}

		public void SetComboBox()
		{
			try
			{
				string[] directorieStrings = Directory.GetFiles(FileTools.logDirPath);

				int cnt = 0;
				foreach (string s in directorieStrings)
				{
					Log_File_ComboBox.Items.Add(s);
					++cnt;
				}
				Log_File_ComboBox.SelectedIndex = cnt - 1;
			}
			catch (ArgumentNullException)
			{
				//FileTools.WriteLineFile(DateTime.Now.ToString() + FileTools.exceptionFilePath, " 路径为空！！");
				FileTools.WriteLineFile(FileTools.exceptionFilePath, " 路径为空！！");
			}
            catch (DirectoryNotFoundException)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, " 路径为空！！");
			}
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
					Log_ListView.Items.Add(new { date = data[0], number = data[1], template = data[2], printer = data[3], success = data[4] });
				}

				streamReader.Close();
			}
			catch (FileNotFoundException)
			{
				FileTools.WriteLineFile(FileTools.exceptionFilePath, "未找到文件！");
			}
			catch(DirectoryNotFoundException)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, "未找到文件！");
			}
		}

		//写记录到listView
		public void WriteDataToListView(string[] data)
		{
			string date = Log_File_ComboBox.SelectedItem.ToString();
			string todayFile = FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			if (date == todayFile && data.Length == 4)
			{
				Log_ListView.Items.Add(new { date = data[0], number = data[1], template = data[2], printer = data[3] });
			}
		}

		private void LogFileComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Log_ListView.Items.Clear();
			LoadData(Log_File_ComboBox.SelectedItem.ToString());
		}
		}
	}

