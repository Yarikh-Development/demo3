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
//using System.Windows.FrameworkElement;

namespace demo
{
	public enum PrinterStatus
	{
		其他状态 = 1,
		未知,
		空闲,
		正在打印,
		预热,
		停止打印,
		打印中,
		离线
	}

	public class City
	{
		public int ID { get; set; }
		public string Name { get; set; }
        public PrinterStatus Status { get; set; }
    }

	class Printer
	{
		private static List<City> citys = new List<City>();
		private static ZebraPrinter printerHTTP = null;

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
		public static PrinterStatus GetPrinterStatus(string PrinterDevice)
		{
			PrinterStatus ret = 0;
			string path = @"win32_printer.DeviceId='" + PrinterDevice + "'";
			ManagementObject printer = new ManagementObject(path);
			printer.Get();
			ret = (PrinterStatus)Convert.ToInt32(printer.Properties["PrinterStatus"].Value);
			return ret;
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
					list.Add(new City { ID = ++cnt, Name = printerName, Status = GetPrinterStatus(printerName) });
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
					list.Add(new City { ID = ++cnt, Name = printerName, Status = GetPrinterStatus(printerName) });
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
			}
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

		/*private void txtPrintersName_TouchDown(object sender, TouchEventArgs e)
        {
            if (itemsPrinters.SelectedIndex < 0) return;

            ListBoxItem myListBoxItem = (ListBoxItem)(NewVideoListBox.ItemContainerGenerator.ContainerFromIndex(NewVideoListBox.SelectedIndex));
            ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            MaterialDesignThemes.Wpf.Badged badged = (MaterialDesignThemes.Wpf.Badged)myDataTemplate.FindName("CountingBadge", myContentPresenter);

            TextBlock likeText = (TextBlock)myDataTemplate.FindName("LikeText", myContentPresenter);

            likeText.Foreground = new SolidColorBrush(Colors.Red);



            if (badged.Badge == null)
                badged.Badge = 0;
            int i;
            int.TryParse(badged.Badge.ToString(), out i);
            badged.Badge = i + 1;
        }*/
	}
}
