using Microsoft.Win32;
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
using Zebra.Sdk.Comm;

namespace demo
{
    /// <summary>
    /// NormalPrinterSetting.xaml 的交互逻辑
    /// </summary>
    public partial class NormalPrinterSetting : Window
    {
        string printerName = "";
        public NormalPrinterSetting(string printerName,string buttonName)
        {
            InitializeComponent();
            this.printerName = printerName;
            txtPrinterName.Text = printerName;
            RadioButton radioButton = panelMenu.FindName(buttonName) as RadioButton;
            radioButton.IsChecked = true;
            PrinterHeadStatus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if((bool)btnSendFile.IsChecked)
            {
                gridSendFile.Visibility = Visibility.Visible;
                gridHighSetting.Visibility = Visibility.Hidden;
                gridMessage.Visibility = Visibility.Hidden;
            }
            if ((bool)btnPrintSetting.IsChecked)
            {
                gridSendFile.Visibility = Visibility.Hidden;
                gridHighSetting.Visibility = Visibility.Hidden;
                gridMessage.Visibility = Visibility.Hidden;
            }
            if ((bool)btnBasicMessage.IsChecked)
            {
                gridSendFile.Visibility = Visibility.Hidden;
                gridHighSetting.Visibility = Visibility.Hidden;
                gridMessage.Visibility = Visibility.Visible;
            }
            if ((bool)btnHighSetting.IsChecked)
            {
                gridSendFile.Visibility = Visibility.Hidden;
                gridHighSetting.Visibility = Visibility.Visible;
                gridMessage.Visibility = Visibility.Hidden;
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMiniWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnSendFile_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Visible;
            gridHighSetting.Visibility = Visibility.Hidden;
            gridMessage.Visibility = Visibility.Hidden;
        }

        private void btnBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Hidden;
            gridMessage.Visibility = Visibility.Visible;
        }

        private void btnPrintSet_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Hidden;
            gridMessage.Visibility = Visibility.Hidden;
        }

        private void btnHigtSet_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Visible;
            gridMessage.Visibility = Visibility.Hidden;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();//声明类，用于打开文件
                openFile.Filter = "TestOpenWrite(*.zpl)|*.zpl";//文件过滤器
                openFile.ShowDialog();//调用系统文件夹展示文件列表，用于查找文件
                txtFilePath.Text = openFile.FileName;//将获取到的文件名赋值
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSendPath_Click(object sender, RoutedEventArgs e)
        {
            string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            bool zpl = false;
            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
            //发送文件
            if (txtFilePath.Text != "")
            {
                //bool zpl = Printers.SendFile(FileTools.labelDirPath + "\\" + data[1] + ".zpl"); //发送ZPL 
                zpl = Printer.SendFile(txtFilePath.Text);
            }
            else
            {
                MessageBox.Show("文件路径为空");
                Printer.ClosePrinter();
                return;
            }
            if (!zpl)
                MessageBox.Show("文件发送失败！");
            Printer.ClosePrinter();
        }

        private void btnSendOrder_Click(object sender, RoutedEventArgs e)
        {
            string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            bool zpl = false;
            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
            //发送指令
            if (txtOrder.Text != "")
            {
                zpl = Printer.SendPrinterCommand(txtOrder.Text);
            }
            else
            {
                MessageBox.Show("指令为空");
                Printer.ClosePrinter();
                return;
            }
            if (!zpl)
                MessageBox.Show("指令发送失败！");
            Printer.ClosePrinter();
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            if (txtServerName.Text == "" || txtDNSServer.Text == "")
            {
                MessageBox.Show("请正确填写内容！");
                return;
            }
            string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            bool zpl = false;
            string command = "! U setvar \"weblink.ip.conn1.location\" \"" +
                txtServerName + "\"\r\n! U setvar \"internal_wired.ip.dns.servers\" \"" +
                txtDNSServer + "\"\r\n! U setvar \"device.reset\" \"\" ";
            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
            //发送指令
            zpl = Printer.SendPrinterCommand(command);
            if (!zpl)
                MessageBox.Show("指令发送失败！");
            Printer.ClosePrinter();
        }

        private void PrinterHeadStatus()
        {
            Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            if (Printer.IsPrinterOnline())
            {
                //警告为红色字体
                if (Printer.IsHeadOpen())
                {
                    txtIsHeadOpen.Text = "打印头打开";
                    txtIsHeadOpen.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                else
                {
                    txtIsHeadOpen.Text = "打印头正常";
                }
                if (Printer.IsHeadTooHot())
                {
                    txtIsTooHot.Text = "打印头过热";
                    txtIsTooHot.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                else
                {
                    txtIsTooHot.Text = "温度正常";
                }
                if (Printer.IsPaperOut())
                {
                    txtIsPaperOut.Text = "缺纸";
                    txtIsPaperOut.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                else
                {
                    txtIsPaperOut.Text = "纸张正常";
                }
                if (Printer.IsRibbonOut())
                {
                    txtIsRibbonOut.Text = "碳带缺失";
                    txtIsRibbonOut.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                else
                {
                    txtIsRibbonOut.Text = "碳带正常";
                }
                txtLableLength.Text = Printer.LabelLength().ToString();
                txtLabelRemain.Text = Printer.LabelsRemainingInBatch().ToString();
                txtPrinterMode.Text = Printer.PrinterMode().ToString();
            }
            else
            {
                txtIsHeadOpen.Text = "";
                txtIsTooHot.Text = "";
                txtIsPaperOut.Text = "";
                txtIsRibbonOut.Text = "";
                txtLableLength.Text = "";
                txtLabelRemain.Text = "";
                txtPrinterMode.Text = "";
            }
            Printer.ClosePrinter();
        }

        private void TestAllSettings_Click(object sender, RoutedEventArgs e)
        {
            Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            if (Printer.GetPrinterSettingValus() != null)
                MessageBox.Show(Printer.GetPrinterSettingValus());
            else
                MessageBox.Show("连接失败！");
            //Printer.GetPrinterAllSetting();
            Printer.ClosePrinter();
        }
    }
}
