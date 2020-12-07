using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace demo
{
    public class AddPrintersMessage
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Status { get; set; }
        public string IP { get; set; }

    }
    /// <summary>
    /// AddPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class AddPrinter : Window
    {
        private ObservableCollection<AddPrintersMessage> printIP = null;
        private Printer printer = null;
        List<City> list = null;
        public AddPrinter()
        {
            InitializeComponent();
            printIP = new ObservableCollection<AddPrintersMessage>();
            itemsPrinters.ItemsSource = printIP;
            list = Printer.SetPrinters();
        }

        private void inputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputValue = inputText.Text;
                if (inputText.Text == "") return;
                //var vm = this.DataContext;
                foreach (var item in list)
                {
                    if (inputValue == item.Port)
                    {
                        printIP.Add(new AddPrintersMessage { IP = inputValue, Name = item.Name});
                    }
                }
                if (printIP == null)
                    txtMessage.Text = "找不到打印机";
                //添加内容
                
                //添加内容后将文本框清空
                inputText.Text = string.Empty;
            }
        }
    }
}
