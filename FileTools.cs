using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;

namespace demo
{
	class FileTools
	{
		
		//private static readonly string Base = Directory.GetParent(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase).Parent.FullName;
		private static readonly string Base = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
		public static string relationFilePath { get; private set; } = Base + "\\relation.txt";
		public static string labelDirPath { get; private set; } = Base + "\\labelLib";
		public static string pictureDirPath { get; private set; } = Base + "\\picture";
		public static string logDirPath { get; private set; } = Base + "\\log";
		public static string exceptionFilePath { get; private set; } = Base + "\\exception.txt";
		public static string settingFilePath { get; private set; } = Base + "\\setting.txt";

		//读取设置文件，读取相关路径进行初始化
		public static void Init()
		{
			FileStream fileStream = null;
			StreamReader settingReader = null;
			try
            {
				fileStream = new FileStream(settingFilePath, FileMode.OpenOrCreate, FileAccess.Read);
				settingReader = new StreamReader(fileStream);
				new FileStream(exceptionFilePath, FileMode.OpenOrCreate, FileAccess.Read).Close();

				string[] pathList = settingReader.ReadToEnd().Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

				if (pathList.Length != 4)
				{
					settingReader.Close();
					fileStream.Close();
					SaveSettingFile(settingFilePath, new string[] { relationFilePath, labelDirPath, pictureDirPath, logDirPath });
					return;
				}
				
				relationFilePath = pathList[0];
				labelDirPath = pathList[1];
				pictureDirPath = pathList[2];
				logDirPath = pathList[3];
			}
			catch(FileNotFoundException)
            {
				SaveSettingFile(settingFilePath, new string[] { relationFilePath, labelDirPath, pictureDirPath, logDirPath });
			}
			catch(IOException e)
            {
				MessageBox.Show(e.ToString());
			}
			finally
            {
				settingReader.Close();
				fileStream.Close();
			}
		}

		//保存设置到文件
		public static void SaveSettingFile(string settingFilePath, string[] path)
        {
			//先清空文件
			FileStream stream = File.Open(settingFilePath, FileMode.OpenOrCreate, FileAccess.Write);
			stream.Seek(0, SeekOrigin.Begin);
			stream.SetLength(0);
			stream.Close();
			//写入文件
			StreamWriter settingWriter = new StreamWriter(settingFilePath);
			foreach (string p in path)
            {
				settingWriter.WriteLine(p);
			}

			//更新内存
			relationFilePath = path[0];
			labelDirPath = path[1];
			pictureDirPath = path[2];
			logDirPath = path[3];

			settingWriter.Close();
		}

		//打开目录对话框
		public static string OpenFolder()
		{
			System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
			fbd.ShowDialog();
			return fbd.SelectedPath;
		}

		//打开文件对话框
		public static string OpenFile(string ExtensionName, string Default)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.DefaultExt = Default;
			dlg.Filter = ExtensionName;
			dlg.ShowDialog();

			return dlg.FileName;
		}

		//获取短文件名
		public static string GetFileShortName(string path)
        {
			string[] arr = path.Split(new char[] { '\\' });
			if (arr.Length != 0)
				return arr[arr.Length - 1];
			return "";
        }

		//写数据到文件
		public static void WriteLineFile(string path, string data)
        {
            try
            {
				FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.WriteLine(data);
				streamWriter.Close();
			}
			catch(IOException)
            {
				StreamWriter streamWriter = new StreamWriter(exceptionFilePath);
				streamWriter.WriteLine(DateTime.Now.ToString() + path + " " + " 文件写入失败！数据：" + data);
				streamWriter.Close();
			}
        }

		//删除指定行
		public static void DeleteLine(string path, int line)
        {
			try
			{
				List<string> lines = new List<string>(File.ReadAllLines(path));
				lines.RemoveAt(line);
				File.WriteAllLines(path, lines.ToArray());
			}
			catch (IOException)
			{
				StreamWriter streamWriter = new StreamWriter(exceptionFilePath);
				streamWriter.WriteLine(DateTime.Now.ToString() + path + " " + " 文件删除失败！行：" + line);
				streamWriter.Close();
			}
		}

		//获取指定行
		public static string ReadLine(string path, int line)
        {
			List<string> lines = new List<string>(File.ReadAllLines(path));
			return lines[line];
		}

		//覆盖指定行
		public static void CoverLine(string path, string data, int line)
        {
			List<string> lines = new List<string>(File.ReadAllLines(path));
			lines[line] = data;
			File.WriteAllLines(path, lines.ToArray());
		}

		//查找目录中是否有含关键字的文件名
		public static string FindFiles(string dirPath, string keyword, string Extension)
        {
			string[] directorieStrings = Directory.GetFiles(dirPath);

			for (int cnt = 0; cnt < directorieStrings.Length; ++cnt)
			{
				if (directorieStrings[cnt].Contains(keyword) && 
					(Path.GetExtension(directorieStrings[cnt]) == Extension))

					return directorieStrings[cnt];
			}

			return "";
		}
	}
}
