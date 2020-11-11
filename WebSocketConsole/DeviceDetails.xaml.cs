using System;
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
using System.Windows.Shapes;

namespace WebSocketConsole
{
    /// <summary>
    /// DeviceDetails.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDetails : Page
    {
        MainWindow frmMainmF = null;
        public DeviceDetails(MainWindow myWindow)
        {
            InitializeComponent();
            this.frmMainmF = myWindow;
        }
        public DeviceDetails()
        {
            InitializeComponent();
            
        }

        /*private void FrmDeviceDetails_Load(object sender, EventArgs e)
        {
            txtDataToSend.Text = "";
            txtReceived.Text = "";
            txtSendToDeviceSN.Text = frmMainmF.msCurrentSelectedDeviceSN;
        }*/

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtDataToSend.Text = "";
            txtReceived.Text = "";
            txtSendToDeviceSN.Text = frmMainmF.msCurrentSelectedDeviceSN;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (txtDataToSend.Text == "")
            {
                MessageBox.Show("Please input the data you want to send to the printer!", this.Title, MessageBoxButton.OK);
                txtDataToSend.Focus();
                return;
            }

            if (txtSendToDeviceSN.Text == "")
            {
                MessageBox.Show("Please input the serial number of the printer you want to send to!", this.Title, MessageBoxButton.OK);
                txtSendToDeviceSN.Focus();
                return;
            }
            txtReceived.Text = frmMainmF.SendRawDataToPrinter(txtDataToSend.Text, txtSendToDeviceSN.Text);
        }

    }
}
