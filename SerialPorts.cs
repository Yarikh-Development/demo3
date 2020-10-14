using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace demo
{
    class SerialPorts
    {
        public SerialPort serialPort = new SerialPort();//初始化SerialPort实例
        /// <summary>
        /// 获取本机串口列表
        /// </summary>
        /// <param name="isUseReg"></param>
        /// <returns></returns>
        private List<string> GetComlist(bool isUseReg)
        {
            List<string> list = new List<string>();
            try
            {
                if (isUseReg)
                {
                    RegistryKey RootKey = Registry.LocalMachine;
                    RegistryKey Comkey = RootKey.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");

                    String[] ComNames = Comkey.GetValueNames();

                    foreach (String ComNamekey in ComNames)
                    {
                        string TemS = Comkey.GetValue(ComNamekey).ToString();
                        list.Add(TemS);
                    }
                }
                else
                {
                    foreach (string com in SerialPort.GetPortNames())  //自动获取串行口名称  
                        list.Add(com);
                }
            }
            catch
            {
                MessageBox.Show("串行端口检查异常！", "提示信息");
                // System.Environment.Exit(0); //彻底退出应用程序   
            }
            return list;
        }

        /// <summary>
        /// 判断是否存在当前串口
        /// </summary>
        private void StartSerialPortMonitor()
        {
            List<string> comList = GetComlist(false); //首先获取本机关联的串行端口列表     
            if (comList.Count == 0)
            {
                MessageBox.Show("当前设备不存在串行端口！", "提示信息");
                // System.Environment.Exit(0); //彻底退出应用程序   
            }
            else
            {
                string targetCOMPort = "COM4";
                //判断串口列表中是否存在目标串行端口
                if (!comList.Contains(targetCOMPort))
                {
                    MessageBox.Show("提示信息", "当前设备不存在配置的串行端口！");
                    //System.Environment.Exit(0); //彻底退出应用程序   
                }
            }
        }

        /// <summary>
        /// 设置通讯串口
        /// </summary>
        public void setcom()
        {
            try
            {
                StartSerialPortMonitor();
                serialPort.PortName = "COM4"; //通信端口
                serialPort.BaudRate = 115200; //串行波特率
                serialPort.DataBits = 8; //每个字节的标准数据位长度
                serialPort.StopBits = StopBits.One; //设置每个字节的标准停止位数
                serialPort.Parity = Parity.None; //设置奇偶校验检查协议
                //下面这句是当信息中有汉字时，能正确传输，不然会出现问号。
                serialPort.Encoding = System.Text.Encoding.GetEncoding("GB2312");
                //串口控件成员变量，字面意思为接收字节阀值，
                //串口对象在收到这样长度的数据之后会触发事件处理函数
                //一般都设为1
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(CommDataReceived); //设置数据接收事件（监听）

                serialPort.Open(); //打开串口
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 通讯有数据进执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CommDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            getcom();
        }

        public void getcom()
        {
            try
            {
                //定义一个字段，来保存串口传来的信息。
                string str = "";

                int len = serialPort.BytesToRead;
                Byte[] readBuffer = new Byte[len];
                serialPort.Read(readBuffer, 0, len);
                str = Encoding.Default.GetString(readBuffer);


                //如果需要和界面上的控件交互显示数据，使用下面的方法。其中ttt是控件的名称。
                //this.tttt.Dispatcher.Invoke(new Action(() =>
                //{
                //    tttt.Text = str ;
                //}));


                serialPort.DiscardInBuffer();  //清空接收缓冲区     
            }
            catch (Exception ex)
            {
                serialPort.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow1_Closed(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}
