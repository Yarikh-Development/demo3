using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace WebSocketConsole
{
    public class CommandContent
    {
        public string Direct { get; set; }
        public string Explain { get; set; }
    }
    /// <summary>
    /// DeviceDetails.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDetails : Page
    {
        MainWindow frmMainmF = null;
        
        private string printerSN = "";
        private ObservableCollection<CommandContent> commandContentZPL = null;
        private ObservableCollection<CommandContent> commandContentSGD = null;
        private string filePath = "";
        public DeviceDetails(MainWindow myWindow,string printerSN,string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.frmMainmF = myWindow;
            this.printerSN = printerSN;
        }
        public DeviceDetails()
        {
            InitializeComponent();
            
        }

        private void ZPL_Click(object sender, RoutedEventArgs e)
        {
            this.ZPL.Visibility = Visibility.Visible;
            this.SGD.Visibility = Visibility.Hidden;
        }
        private void SGD_Click(object sender, RoutedEventArgs e)
        {
            this.SGD.Visibility = Visibility.Visible;
            this.ZPL.Visibility = Visibility.Hidden;
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
            txtSendToDeviceSN.Text = printerSN;

            try
            {
                commandContentZPL = new ObservableCollection<CommandContent>();
                string filePathZPL = filePath + "\\commandZPL.txt";
                FileStream fileStreamZPL = new FileStream(filePathZPL, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader streamReaderZPL = new StreamReader(fileStreamZPL);

                commandContentSGD = new ObservableCollection<CommandContent>();
                string filePathSGD = filePath + "\\commandSGD.txt";
                FileStream fileStreamSGD = new FileStream(filePathSGD, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader streamReaderSGD = new StreamReader(fileStreamSGD);

                string line;
                string[] data;
                while ((line = streamReaderSGD.ReadLine()) != null)
                {
                    data = line.Split(';');
                    commandContentSGD.Add(new CommandContent { Direct = data[0], Explain = data[1] });
                }
                SGD.ItemsSource = commandContentSGD;

                while ((line = streamReaderZPL.ReadLine()) != null)
                {
                    data = line.Split(';');
                    commandContentZPL.Add(new CommandContent { Direct = data[0], Explain = data[1] });
                }
                ZPL.ItemsSource = commandContentZPL;

                streamReaderZPL.Close();
                streamReaderSGD.Close();
            }
            catch (FileNotFoundException)
            {
                //FileTools.WriteLineFile(FileTools.exceptionFilePath, "未找到指令文件！");
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                //FileTools.WriteLineFile(FileTools.exceptionFilePath, " 指令文件路径为空！！");
                throw;
            }
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

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            commandContentZPL.Clear();
            commandContentSGD.Clear();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var c = btn.DataContext as CommandContent;
            txtDataToSend.Text = c.Direct;
            //MessageBox.Show(c.Explain);
        }
    }
}
