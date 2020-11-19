using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;

namespace demo
{
    class PrinterZebra
    {
		Printer normalPrinter = null;
		private static ZebraPrinter printer = null;
		public static string LinkPrinter(string printerName, int port)
		{
			try
			{
				string ip = Printer.GetRegistryData(printerName + "\\DsSpooler", "portName");

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
				printer = ZebraPrinterFactory.GetInstance(connection);
			}
			catch (ConnectionException e)
			{
				printer = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + e.Message);
				return printerName + e.Message;
			}
			catch (FormatException e)
			{
				printer = null;
				FileTools.WriteLineFile(FileTools.exceptionFilePath, DateTime.Now.ToString() + " " + printerName + "IP地址不正确！");
				return printerName + "IP地址不正确！";
			}

			return "";
		}

		//发送文件
		public static bool SendFile(string filePath)
		{
			if (printer != null && filePath != "")
			{
				printer.SendFileContents(filePath);
				return true;
			}
			return false;
		}

		//关闭链接
		public static void ClosePrinter()
		{
			if (printer != null)
			{
				printer.Connection.Close();
				printer = null;
			}
		}
	}
}
