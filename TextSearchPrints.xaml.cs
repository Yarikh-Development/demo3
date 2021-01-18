using demo.ClassFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextSearchPrints.xaml 的交互逻辑
    /// </summary>
    public partial class TextSearchPrints : Page
    {
        Printer printer;
        private string printerName = "";
        private AddPrinter add = null;
        //private ObservableCollection<AddPrintersMessage> executePrinters = null;
        public TextSearchPrints(Window window)
        {
            InitializeComponent();
            //Printer.SetPrinters();
            Printer.SetPrinters(itemsPrinters);
            printersCount.Text =  Printer.printerCount.ToString();
            //txtPrintersState.Text = Printer.GetPrinterStatus()
            //AddExecutePrinters();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //读取打印机列表的第一项
            printer = new Printer();
            isVisibilityForEnter.Visibility = Visibility.Visible;
            List<Button> buttonItems = printer.GetChildObjects<Button>(itemsPrinters, "");
            if (buttonItems.Count > 0)
            {
                Button button = buttonItems.First();
                TextBlock txt = printer.FindFirstVisualChild<TextBlock>(button, "txtPrintersName");
                txtPrinterName2.Text = txt.Text;
                
            }
            

        }

        //展示ItemsControl里的打印机名
        private void printerMessage_Click(object sender, RoutedEventArgs e)
        {
            printer = new Printer();
            isVisibilityForEnter.Visibility = Visibility.Visible;
            List<Button> buttonItems = printer.GetChildObjects<Button>(itemsPrinters, "");            
            foreach (Button button in buttonItems)
            {                
                if (button.IsFocused)
                {
                    TextBlock txt = printer.FindFirstVisualChild<TextBlock>(button, "txtPrintersName");
                    txtPrinterName2.Text = txt.Text;
                    printerName = txt.Text;
                }                   
            }
        }

        private void printerMessage_Initialized(object sender, EventArgs e)
        {

        }

        private void btnSentFile_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName, btnSendFile.Name);
            normalPrinter.ShowDialog();
        }

        private void btnHighSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName, btnHighSetting.Name);
            normalPrinter.ShowDialog();
        }

        private void btnPrintSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName, btnPrintSetting.Name);
            normalPrinter.ShowDialog();
            
            
        }

        private void btnBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(printerName, btnBasicMessage.Name);
            normalPrinter.ShowDialog();
        }

        private void btnRafresh_Click(object sender, RoutedEventArgs e)
        {
            Printer.SetPrinters(itemsPrinters);
            printersCount.Text = Printer.printerCount.ToString();
        }

        private void btnAddPrinter_Click(object sender, RoutedEventArgs e)
        {
            add = new AddPrinter();
            add.ShowDialog();
        }
    }
}
