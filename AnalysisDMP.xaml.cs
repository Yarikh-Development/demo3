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
using System.IO;
using Microsoft.Win32;
using WebSockets;
//using System.Windows.Forms;

namespace demo
{
    /// <summary>
    /// AnalysisDMP.xaml 的交互逻辑
    /// </summary>
    public partial class AnalysisDMP : Window
    {
        //WebServer server;
        WebSocketConsole.MainWindow frmMainmF = null;
        AnalysisOrderResult analysisOrderResult = null;
        String OrderResult = "";
        String SN = "";
        String DMPFilePath = "";
        public AnalysisDMP(WebSocketConsole.MainWindow window)
        {
            InitializeComponent();
            this.frmMainmF = window;
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            PanelList.Visibility = Visibility.Visible;
            panelSetting.Visibility = Visibility.Hidden;
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            PanelList.Visibility = Visibility.Hidden;
            panelSetting.Visibility = Visibility.Visible;
        }

        private void btnFindFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpenPath_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFile = new OpenFileDialog();
            System.Windows.Forms.FolderBrowserDialog openFile = new System.Windows.Forms.FolderBrowserDialog();
            openFile.ShowDialog();
            DMPFilePath = openFile.SelectedPath;
            txtPath.Text = openFile.SelectedPath;
        }

        private void btnGetPrinterFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //frmMainmF = new WebSocketConsole.MainWindow();
                if (txtSN.Text != "")
                {
                    SN = txtSN.Text;
                    //这里需要遍历一次LinkOSPrinters里的SN属性，是否存在这个SN码
                    OrderResult = frmMainmF.SendRawDataToPrinter("^XA^HWE:^XZ", txtSN.Text);
                    txtPrinterFile.Text = OrderResult;
                }
                else
                {
                    MessageBox.Show("SN为空，请输入SN码！", this.Title, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);              
            }
            
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            if(DMPFilePath != "")
            {
                analysisOrderResult = new AnalysisOrderResult(OrderResult, SN, frmMainmF);
                analysisOrderResult.DMPFile(DMPFilePath);
                listView.ItemsSource = analysisOrderResult.DMPs;
            }
            else
            {
                MessageBox.Show("路径为空，请输入路径！", this.Title, MessageBoxButton.OK);
            }
            
        }

        private void menuPreview_Click(object sender, RoutedEventArgs e)
        {
            // Grid.SetColumnSpan(menuPreview, 1);
            DMPFile dMPFile = listView.SelectedItem as DMPFile;
            //StreamReader reader = new StreamReader(DMPFilePath + "//" + dMPFile.FileName, Encoding.Default);
            MessageBox.Show(File.ReadAllText(DMPFilePath + "//" + dMPFile.FileName));
            //MessageBox.Show(reader.Read().ToString());
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpenBasePath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCloseContent_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
