using System;
using System.Collections;
using System.Collections.Generic;
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

namespace demo.View.NomalView
{
    /// <summary>
    /// DeviceDetailsNomalView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDetailsNomalView : Page
    {
        private string _port;
        public DeviceDetailsNomalView(string printerPort)
        {
            _port = printerPort;
            
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtSendToDeviceIP.Text = _port;
        }

        private void btnSendTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSendToDeviceIP.Text == "")
                {
                    MessageBox.Show("请输入正确的IP地址！");
                }
                string[] strArray = (string[])Printer.FormatSGD(txtDataToSend.Text).ToArray(typeof(string));

                if (strArray.Length == 1)
                {
                    txtReceived.Text = Printer.GetPrinterSettingValus(strArray[0]);
                }
                else if (strArray.Length == 2)
                {
                    bool isSendTo = Printer.SetPrintSetting(strArray[0], strArray[1]);
                    txtReceived.Text = "";
                    if (!isSendTo)
                        txtReceived.Text = "指定打印机不在线！";

                }
                else
                {
                    txtReceived.Text = "请正确输入指令!";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void ZPL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SGD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Printer.ClosePrinter();
        }
    }
}
