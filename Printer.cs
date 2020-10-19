using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Controls;

namespace demo
{
	public class City
	{
		//测试上传
		//测试上传2
		public int ID { get; set; }
		public string Name { get; set; }
	}

	class Printer
	{
		private static List<City> citys = new List<City>();
		public static List<City> Citys
        {
			get { return citys; }
            set { citys = value; }
        }
		private static PrintDocument printer = new PrintDocument();

		public static void GetPrinterInfo()
		{
			
		}

		//发送文件
		public static bool SendFile(string printerName, string filePath)
		{
			try
			{
				if (printer != null && filePath != "")
				{
					PrintDocument pd = new PrintDocument();
					pd.DefaultPageSettings.PrinterSettings.PrinterName = printerName;
					
					pd.DefaultPageSettings.Landscape = false;
					pd.PrintPage += (sender, args) =>
					{
						//从指定文件里创建图片
						System.Drawing.Image i = System.Drawing.Image.FromFile(filePath);
						args.Graphics.DrawImageUnscaled(i, 0, 0);
					};
					pd.Print();
				}
			}
			catch (InvalidPrinterException)
			{
				FileTools.WriteLineFile(FileTools.exceptionFilePath, printerName + "无效的打印机设置！");
				return false;
			}
			catch(System.ComponentModel.Win32Exception)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, printerName + "给打印机的数据过小(访问拒绝)！");
				return false;
			}
			catch(System.IO.FileNotFoundException)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, printerName + "没找到指定文件或目录！");
				return false;
			}
			return true;
		}

		//判断是否为斑马打印机
		public static bool IsZebraPrinter(string printerName)
        {
			string name = GetRegistryData(printerName + "\\PnPData", "HardwareID");
			return name.ToLower().Contains("zebra");
		}

		//从注册表获取打印机数据
		public static string GetRegistryData(string subKey, string name)
        {
			string portQuery = @"System\CurrentControlSet\Control\Print\Printers\" + subKey;

			RegistryKey portKey = Registry.LocalMachine.OpenSubKey(portQuery, RegistryKeyPermissionCheck.Default,
								  System.Security.AccessControl.RegistryRights.QueryValues);
			if (portKey != null)
			{
				object IPValue = portKey.GetValue(name, String.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames);
				if (IPValue == null)
					return "";
				else if (IPValue.ToString() != "System.String[]")
					return IPValue.ToString();
				else
					return ((string[])IPValue)[0];
			}

			return "";
		}

		//获取斑马打印机列表
		public static void SetPrinters(ComboBox comboBox)
        {
			int cnt = 0;
			List<City> list = new List<City>();
			foreach (string printerName in PrinterSettings.InstalledPrinters)
			{
				if (IsZebraPrinter(printerName))
				{
					list.Add(new City { ID = ++cnt, Name = printerName });
				}
			}
			comboBox.ItemsSource = list;
			comboBox.SelectedIndex = 0;
		}

		//获取斑马打印机列表
		public static void SetPrinters(ItemsControl itemsControl)
		{
			int cnt = 0;
			List<City> list = new List<City>();
			foreach (string printerName in PrinterSettings.InstalledPrinters)
			{
				if (IsZebraPrinter(printerName))
				{
					list.Add(new City { ID = ++cnt, Name = printerName });
				}
			}
			itemsControl.ItemsSource = list;

		}

		//返回打印机列表
		public static List<City> SetPrinters()
		{
			int cnt = 0;
			List<City> list = new List<City>();
			foreach (string printerName in PrinterSettings.InstalledPrinters)
			{
				if (IsZebraPrinter(printerName))
				{
					list.Add(new City { ID = ++cnt, Name = printerName });
				}
			}
			return list;

		}

		/// <summary>
		/// Setprinters的重构，返回一个City的集合
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		/*public static void SetPrinters()
		{
			int cnt = 0;
			//List<City> list = new List<City>();
			foreach (string printerName in PrinterSettings.InstalledPrinters)
			{
				if (IsZebraPrinter(printerName))
				{
					Printer.Citys.Add(new City { ID = ++cnt, Name = printerName });
				}
			}
			
		}*/
	}
}
