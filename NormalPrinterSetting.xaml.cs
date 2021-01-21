using demo.ClassFolder;
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
using System.Windows.Interop;
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
        private string printerName = "";
        private List<string> scale;
        private IntPtr _windowHandle;
        public NormalPrinterSetting(string printerName,string buttonName)
        {
            InitializeComponent();
            this.printerName = printerName;
            txtPrinterName.Text = printerName;
            RadioButton radioButton = panelMenu.FindName(buttonName) as RadioButton;
            radioButton.IsChecked = true;
            GetPrinterAllStatus();
            //获取窗口的句柄
            _windowHandle = new WindowInteropHelper(this).Handle;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if((bool)btnSendFile.IsChecked)
            {
                gridSendFile.Visibility = Visibility.Visible;
            }
            if ((bool)btnPrintSetting.IsChecked)
            {
                gridPrintSetting.Visibility = Visibility.Visible;
            }
            if ((bool)btnBasicMessage.IsChecked)
            {
                gridMessage.Visibility = Visibility.Visible;
            }
            if ((bool)btnHighSetting.IsChecked)
            {
                gridHighSetting.Visibility = Visibility.Visible;
            }
            //GetRibbonScale();
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
            gridPrintSetting.Visibility = Visibility.Hidden;
        }

        private void btnBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Hidden;
            gridMessage.Visibility = Visibility.Visible;
            gridPrintSetting.Visibility = Visibility.Hidden;
        }

        private void btnPrintSet_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Hidden;
            gridMessage.Visibility = Visibility.Hidden;
            gridPrintSetting.Visibility = Visibility.Visible;
        }

        private void btnHigtSet_Click(object sender, RoutedEventArgs e)
        {
            gridSendFile.Visibility = Visibility.Hidden;
            gridHighSetting.Visibility = Visibility.Visible;
            gridMessage.Visibility = Visibility.Hidden;
            gridPrintSetting.Visibility = Visibility.Hidden;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();//声明类，用于打开文件
                openFile.Filter = "选择模板(*.zpl)|*.zpl;*.prn;*.dmp;*.btw";//文件过滤器
                openFile.ShowDialog();//调用系统文件夹展示文件列表，用于查找文件
                txtFilePath.Text = openFile.FileName;//将获取到的文件名赋值
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
        }

        private void btnSendPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                bool zpl = false;
                if (result != "")
                {
                    txtLogMessage.Text = result;
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
                    txtLogMessage.Text = "文件路径为空！";
                    return;
                }
                if (!zpl)
                    txtLogMessage.Text = "文件发送失败！";
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
            finally
            {
                Printer.ClosePrinter();
            }
           
        }

        private void btnSendOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                bool zpl = false;
                if (result != "")
                {
                    txtLogMessage.Text = result;
                    return;
                }
                //发送指令
                if (txtOrder.Text != "")
                {
                    zpl = Printer.SendPrinterCommand(txtOrder.Text);
                }
                else
                {
                    txtLogMessage.Text = "指令为空！";
                    return;
                }
                if (!zpl)
                    txtLogMessage.Text = "指令发送失败！";
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
            finally
            {
                Printer.ClosePrinter();
            }
            
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtServerName.Text == "" || txtDNSServer.Text == "")
                {
                    txtLogMessage.Text = "请正确填写内容！";
                    return;
                }
                string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                bool zpl = false;
                string command = "! U1 setvar \"weblink.ip.conn1.location\" \"" +
                    txtServerName + "\"\r\n! U1 setvar \"internal_wired.ip.dns.servers\" \"" +
                    txtDNSServer + "\"\r\n! U1 setvar \"device.reset\" \"\" ";
                if (result != "")
                {
                    txtLogMessage.Text = result;
                    return;
                }
                //发送指令
                zpl = Printer.SendPrinterCommand(command);
                if (!zpl)
                    txtLogMessage.Text = "指令发送失败！";
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
            finally
            {
                Printer.ClosePrinter();
            }
            
        }

        private void GetPrinterAllStatus()
        {
            try
            {
                string result = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                if (result != "")
                {
                    txtLogMessage.Text = result;
                    return;
                }
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
                    //txtPrinterMode.Text = Printer.PrinterMode().ToString();
                    
                    //以下这部分注释掉的代码是放弃掉的打印机设置代码功能，说不定哪天还得用上就不删除了
                    //打印速度
                    /*string printSpeed = Printer.GetPrinterSettingValus("media.speed");
                    for (int i = 0; i < cbbPrintSpeed.Items.Count; i++)
                    {
                        if (((ComboBoxItem)cbbPrintSpeed.Items[i]).Content.ToString() == printSpeed)
                        {
                            cbbPrintSpeed.SelectedIndex = i;
                        }
                    }*/
                    //打印浓度
                    /*txtPrintRibbon.Text = Printer.GetPrinterSettingValus("print.tone_zpl");*/
                    //打印模式-操作模式
                    /*string printMode = Printer.PrinterMode().ToString();
                    for (int i = 0; i< cbbPrintMode.Items.Count;i++)
                    {
                        if (((ComboBoxItem)cbbPrintMode.Items[i]).Content.ToString() == printMode)
                        {
                            cbbPrintMode.SelectedIndex = i;
                        }
                    }*/
                    
                }
                else
                {
                    txtIsHeadOpen.Text = "";
                    txtIsTooHot.Text = "";
                    txtIsPaperOut.Text = "";
                    txtIsRibbonOut.Text = "";
                    txtLableLength.Text = "";
                    txtLabelRemain.Text = "";
                    //txtPrinterMode.Text = "";
                }
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
            finally
            {
                Printer.ClosePrinter();
            }
            
            
        }

        private void TestAllSettings_Click(object sender, RoutedEventArgs e)
        {
            /*Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            if (Printer.GetPrinterSettingValus() != null)
                MessageBox.Show(Printer.GetPrinterSettingValus());
            else
                MessageBox.Show("连接失败！");
            //Printer.GetPrinterAllSetting();
            Printer.ClosePrinter();*/
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string password = cbbPassword1.Password + cbbPassword2.Password + cbbPassword3.Password + cbbPassword4.Password;
                string command = "^XA^KP" + password + "^JUS^XZ";
                string message = Printer.LinkPrinter(printerName, TcpConnection.DEFAULT_ZPL_TCP_PORT);
                if (message != "")
                {
                    txtLogMessage.Text = message;
                    return;
                }

                if (Printer.SendPrinterCommand(command))
                {
                    txtLogMessage.Text = "发送成功";
                    txtLogMessage.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    txtLogMessage.Text = "发送失败";
                }
                
            }
            catch (Exception ex)
            {
                txtLogMessage.Text = ex.Message;
            }
            finally
            {
                Printer.ClosePrinter();
            }
            
        }

        /*private void GetRibbonScale()
        {
            scale = new List<string>();
            for (double i = 0;i < 31;i++)
            {
                scale.Add(Convert.ToDouble(i).ToString("0.0"));
            }
            cbbPrintRibbon.ItemsSource = scale;
        }*/

        private void GetPrintMode()
        {
            List<string> printMode = new List<string>();
            printMode.Add("");
        }

        private void txtPrintRibbon_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                return;
                /*e.Handled = true;
                if (e.Key.ToString() != "RightCtrl")
                {
                    MessageBox.Show("输入错误");
                }*/
            }
        }

        private void txtPrintRibbon_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
            /*try
            {
                textBox.Text = textBox.Text.ToString("00.0");
                if (Int64.Parse(textBox.Text) > 30)
                {
                    MessageBox.Show("可以");
                    e.Handled = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("不可以");
                //e.Handled = false;
            }*/
        }

        private void btnDocumentProp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ptrPrompt = 0;
                foreach (var ptr in Printer.SetPrinters())
                {
                    if (ptr.Name.Contains(printerName))
                    {
                        ptrPrompt = ptr.ID;
                    }
                }
                string ptrName = printerName;
                if (ptrName != null && ptrName.Length > 0)
                {
                    IntPtr pPrinter = IntPtr.Zero;
                    IntPtr pDevModeOutput = IntPtr.Zero;
                    IntPtr pDevModeInput = IntPtr.Zero;
                    IntPtr nullPointer = IntPtr.Zero;

                    PtrPage.OpenPrinter(ptrName, ref pPrinter, ref nullPointer);

                    int iNeeded = PtrPage.DocumentProperties(_windowHandle, pPrinter, ptrName, ref pDevModeOutput, ref pDevModeInput, 0);
                    pDevModeOutput = System.Runtime.InteropServices.Marshal.AllocHGlobal(iNeeded);
                    //PtrPage.DocumentProperties(_windowHandle, pPrinter, ptrName, ref pDevModeOutput, ref pDevModeInput, ptrPrompt--);
                    PtrPage.DocumentProperties(_windowHandle, pPrinter, ptrName, ref pDevModeOutput, ref pDevModeInput, 20);
                    PtrPage.ClosePrinter(pPrinter);
                }
            }
            catch (Exception)
            {
                txtLogMessage.Text = "打印机不存在！";
            }
            
        }
    }
}
