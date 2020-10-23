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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketConsole;

namespace demo
{
    /// <summary>
    /// TextHighPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class TextHighPrinter : Page
    {
        DeviceDetails deviceDetails;
        WebSocketConsole.MainWindow mainWindow = new WebSocketConsole.MainWindow();
        public TextHighPrinter()
        {
            InitializeComponent();
        }

        private void printerMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtDialogBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                //skipPages.Content = deviceDetails;
                deviceDetails = new DeviceDetails(mainWindow);
                //deviceDetails.ShowDialog();
                skipPages.Content = deviceDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title,MessageBoxButton.OK);
                
            }
            
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrmSettings frm = new FrmSettings();
                frm.Title = "Settings";
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void btnStartService_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.StartService(btnStartService, picOrange, picGreen);
        }
    }
}
