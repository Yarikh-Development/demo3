using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

namespace demo
{
    /// <summary>
    /// TextSearchPrints.xaml 的交互逻辑
    /// </summary>
    public partial class TextSearchPrints : Page
    {
        Printer printer;
        public TextSearchPrints()
        {
            InitializeComponent();
            //Printer.SetPrinters();
            Printer.SetPrinters(itemsPrinters);
            printersCount.Text =  Printer.SetPrinters().Count.ToString();
            //txtPrintersState.Text = Printer.GetPrinterStatus()

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //int cnt = 0;
            //List<City> list = new List<City>();
            //foreach (string printerName in PrinterSettings.InstalledPrinters)
            //{
            //    if (Printer.IsZebraPrinter(printerName))
            //    {
            //        Printer.Citys.Add(new City { ID = ++cnt, Name = printerName });
            //    }
            //}
            
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
                }                   
            }
        }

        private void printerMessage_Initialized(object sender, EventArgs e)
        {

        }

        private void btnSentFile_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(txtPrinterName2.Text,btnSendFile.Name);
            normalPrinter.ShowDialog();
        }

        private void btnHighSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(txtPrinterName2.Text, btnHighSetting.Name);
            normalPrinter.ShowDialog();
        }

        private void btnPrintSetting_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(txtPrinterName2.Text, btnPrintSetting.Name);
            normalPrinter.ShowDialog();
        }

        private void btnBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            NormalPrinterSetting normalPrinter = new NormalPrinterSetting(txtPrinterName2.Text, btnBasicMessage.Name);
            normalPrinter.ShowDialog();
        }
    }
}
