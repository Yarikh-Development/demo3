using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Management;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Net.NetworkInformation;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using System.IO;
//using System.Windows.FrameworkElement;

namespace demo
{
	/*public enum PrinterStatus
	{
		其他状态 = 1,
		未知,
		空闲,
		正在打印,
		预热,
		停止打印,
		打印中,
		离线
	}*/
	public enum PrinterStatus
	{
		离线,
		在线
	}

	public enum PrinterReadyStatus
    {
		空,
		已准备,
		未准备
    }
	public class City
	{
		public int ID { get; set; }
		public string Name { get; set; }
        public PrinterStatus NetStatus { get; set; }
		public string WareStatus { get; set; }
        public string Port { get; set; }
    }

	class Printer
	{
		private static List<City> citys = new List<City>();
		private static ZebraPrinter printerHTTP = null;
		private static Zebra.Sdk.Printer.PrinterStatus printerStatus = null;
		private static ZebraPrinterLinkOs linkOsPrinter = null;
		private static PrinterStatus ret;
		private static PrinterReadyStatus rets;
		public static int printerCount { get; set; }
        //Zebra.Sdk.Settings.
        //public static bool isPrinterOnline = false;

        public static List<City> Citys
        {
			get { return citys; }
            set { citys = value; }
        }
		private static PrintDocument printer = new PrintDocument();

		public static void GetPrinterInfo()
		{
			
		}

		/// <summary>
		/// 获取打印机状态
		/// </summary>
		/// <param name="PrinterDevice"></param>
		/// <returns></returns>
		/*public static PrinterStatus GetPrinterStatus(string PrinterDevice)
		{
			PrinterStatus ret = 0;
			string path = @"win32_printer.DeviceId='" + PrinterDevice + "'";
			ManagementObject printer = new ManagementObject(path);
			printer.Get();
			ret = (PrinterStatus)Convert.ToInt32(printer.Properties["PrinterStatus"].Value);
			return ret;
		}*/

		//获取网络打印机在线状态
		private static void GetPrinterStatus(string printerName)
        {
            try
            {
				ret = 0;
				rets = 0;
				string result = LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
				if (IsPrinterOnline())
                {
					ret = (PrinterStatus)1;
					if (IsReadyToPrint())
						rets = (PrinterReadyStatus)1;
					else
						rets = (PrinterReadyStatus)2;
				}					
				else
                {
					ret = (PrinterStatus)0;
				}				
			}
            finally
            {
				ClosePrinter();
            }
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

		//获取注册表打印机的端口
		private static string GetprinterPort(string printerName)
        {
			string port = GetRegistryData(printerName + "\\DsSpooler", "portName");
			return port;
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
            try
            {
				int cnt = 0;
				List<City> list = new List<City>();
				string filePathExecutePrinter = FileTools.executePrintersFilePath;
				FileStream fileStreamExecutePrinter = new FileStream(filePathExecutePrinter, FileMode.OpenOrCreate, FileAccess.Read);
				StreamReader streamReaderExecutePrinter = new StreamReader(fileStreamExecutePrinter);
				string line;
				string[] data;
				while ((line = streamReaderExecutePrinter.ReadLine()) != null)
				{
					data = line.Split(';');
					GetPrinterStatus(data[0]);
					list.Add(new City {
						ID = ++cnt, Name = data[0], Port = data[1], NetStatus = ret, WareStatus = rets == 0 ? "" : rets.ToString()
					});
				}
				printerCount = list.Count;
				streamReaderExecutePrinter.Close();
				itemsControl.ItemsSource = list;
			}
            catch (Exception e)
            {
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + e.Message);
            }
			

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
					list.Add(new City { ID = ++cnt, Name = printerName, Port = GetprinterPort(printerName) });
				}
			}
			return list;

		}

		//链接打印机
		public static string LinkPrinter(string printerName, int port)
		{
			try
			{
				string ip = GetRegistryData(printerName + "\\DsSpooler", "portName");

				if (ip == "" || ip.Split(new char[] { '.' }).Length != 4)
					throw new ConnectionException("没找到IP");
				foreach (string s in ip.Split(new char[] { '.' }))
					int.Parse(s);

				Ping pingSender = new Ping();
				PingReply reply = pingSender.Send(ip, 1);//第一个参数为ip地址，第二个参数为ping的时间
				if (reply.Status != IPStatus.Success)
					throw new ConnectionException("链接失败！");

				TcpConnection connection = new TcpConnection(ip, port);
				connection.Open();
				printerHTTP = ZebraPrinterFactory.GetInstance(connection);
				linkOsPrinter = ZebraPrinterFactory.CreateLinkOsPrinter(printerHTTP);
				printerStatus = printerHTTP.GetCurrentStatus();
			}
			catch (ConnectionException e)
			{
				printerHTTP = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + e.Message);
				return printerName + e.Message;
			}
			catch (FormatException e)
			{
				printerHTTP = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + "IP地址不正确！");
				return printerName + "IP地址不正确！";
			}

			return "";
		}

		public static void GetPrinterAllSetting()
        {
			if (printerHTTP != null && linkOsPrinter != null)
            {

				//HashSet<string> availableSettings = linkOsPrinter.GetAvailableSettings();
				Dictionary<string, string> allSettingValues = linkOsPrinter.GetAllSettingValues();
				/*foreach (string setting in availableSettings)
				{
					MessageBox.Show($"{setting}: Range = ({linkOsPrinter.GetSettingRange(setting)})");
				}*/
				foreach (string settingName in allSettingValues.Keys)
				{
					MessageBox.Show($"{settingName}:{allSettingValues[settingName]}");
					//Console.WriteLine($"{settingName}:{allSettingValues[settingName]}");
				}
				//return allSettingValues;
			}
            else
            {
				MessageBox.Show("连接失败！");
            }
			//return null;
		}

		

		//发送文件
		public static bool SendFile(string filePath)
		{
			if (printerHTTP != null && filePath != "")
			{
				printerHTTP.SendFileContents(filePath);
				return true;
			}
			return false;
		}

		//发送指令
		public static bool SendPrinterCommand(string command)
        {
			if (printerHTTP != null && command != "")
            {
				printerHTTP.SendCommand(command);
				return true;
			}			
			return false;
        }

		//关闭链接
		public static void ClosePrinter()
		{
			if (printerHTTP != null)
			{
				printerHTTP.Connection.Close();
				printerHTTP = null;
				printerStatus = null;
				linkOsPrinter = null;
			}
		}
		
		//查看打印机打印头是否打开
		public static bool IsHeadOpen()
        {
			return printerStatus.isHeadOpen;
		}

		//打印机打印头温度是否过高
		public static bool IsHeadTooHot()
        {
			return printerStatus.isHeadTooHot;
        }

		//打印机切纸是否用尽
		public static bool IsPaperOut()
        {
			return printerStatus.isPaperOut;
        }

		//打印机是否准备好
		public static bool IsReadyToPrint()
        {
			return printerStatus.isReadyToPrint;
        }

		//碳带是否用尽
		public static bool IsRibbonOut()
        {
			return printerStatus.isRibbonOut;
        }

		//标签长度
		public static int LabelLength()
        {
			return printerStatus.labelLengthInDots;
        }

		//剩余标签
		public static int LabelsRemainingInBatch()
        {
			return printerStatus.labelsRemainingInBatch;
        }

		//打印模式
		public static ZplPrintMode PrinterMode()
        {
			return printerStatus.printMode;
        }

		//通过指令ID设置打印机（! U1 setvar "input.capture" "run"，其中的"input.capture"为指令ID）
		public static bool SetPrintSetting(string settingID, string value)
        {
			if (printerHTTP != null && linkOsPrinter != null)
			{
				linkOsPrinter.SetSetting(settingID, value);
				return true;
			}
			return false;
        }

		//通过指令ID获取打印机设置信息
		public static string GetPrinterSettingValus(string settingID)
		{
			if (printerHTTP != null && linkOsPrinter != null)
			{
				return linkOsPrinter.GetSettingValue(settingID);
			}
			return "";

		}

		//打印机是否在线
		public static bool IsPrinterOnline()
        {
			return printerHTTP != null && printerStatus != null;
		}

		//查找ItemsControl里的第一个子项
		public T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject

		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T && child.GetValue(FrameworkElement.NameProperty).ToString() == childName)
				{
					return (T)child;
				}
				else
				{
					T childOfChild = FindFirstVisualChild<T>(child, childName);
					if (childOfChild != null)
					{
						return childOfChild;
					}
				}
			}
			return null;
		}

		//查找ItemsControl里的所有子项
		public List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
		{
			DependencyObject child = null;
			List<T> childList = new List<T>();
			for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
			{
				child = VisualTreeHelper.GetChild(obj, i);
				if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
				{
					childList.Add((T)child);
				}
				childList.AddRange(GetChildObjects<T>(child, ""));//指定集合的元素添加到List队尾
			}
			return childList;
		}

		private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is childItem)
					return (childItem)child;
				else
				{
					childItem childOfChild = FindVisualChild<childItem>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}
	}
}
