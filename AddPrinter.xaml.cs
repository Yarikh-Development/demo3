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

namespace demo
{
    public class AddPrintersMessage
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Status { get; set; }
        public string IP { get; set; }

    }
    /// <summary>
    /// AddPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class AddPrinter : Window
    {
        private ObservableCollection<AddPrintersMessage> printIP = null;
        private ObservableCollection<AddPrintersMessage> printName = null;
        private Printer printer = null;
        List<City> list = null;
        private int num = 0;
        public AddPrinter()
        {
            InitializeComponent();
            printIP = new ObservableCollection<AddPrintersMessage>();
            printName = new ObservableCollection<AddPrintersMessage>();
            itemsPrinters.ItemsSource = printIP;
            list = Printer.SetPrinters();
            itemPrinters2.ItemsSource = printName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePathExecutePrinter = FileTools.executePrintersFilePath;
                FileStream fileStreamExecutePrinter = new FileStream(filePathExecutePrinter, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader streamReaderExecutePrinter = new StreamReader(fileStreamExecutePrinter);
                string line;
                string[] data;
                while ((line = streamReaderExecutePrinter.ReadLine()) != null)
                {
                    data = line.Split(';');
                    printName.Add(new AddPrintersMessage { Name = data[0], IP = data[1] });
                }
                txtAddPrinterCount.Text = printName.Count.ToString();
                streamReaderExecutePrinter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        //查找打印机
        private void inputText_KeyDown(object sender, KeyEventArgs e)
        {
            
            txtMessage.Text = "";
            if (e.Key == Key.Enter)
            {
                int num = 0;
                string inputValue = inputText.Text;
                if (inputText.Text == "") return;
                //var vm = this.DataContext;
                foreach (var item in list)
                {
                    foreach (var items in printIP)
                    {
                        if (inputValue == items.IP)
                        {
                            inputText.Text = string.Empty;
                            return;
                        }
                    }
                    if (inputValue == item.Port)
                    {
                        printIP.Add(new AddPrintersMessage { IP = inputValue, Name = item.Name});
                        num++;
                    }
                }
                if (num == 0)
                {
                    txtMessage.Text = "找不到打印机";
                }
                //添加内容后将文本框清空
                inputText.Text = string.Empty;
            }
        }

        private void printerMessage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (printName.Count == 10)
            {
                txtMessage.Text = "打印机超出";
                return;
            }
            printer = new Printer();
            List<Button> buttonItems = printer.GetChildObjects<Button>(itemsPrinters, "");
            foreach (Button button in buttonItems)
            {
                if (button.IsFocused)
                {
                    TextBlock txt = printer.FindFirstVisualChild<TextBlock>(button, "txtPrintersName");
                    foreach (var item in printName)
                    {
                        if (txt.Text == item.Name)
                        {
                            txtMessage.Text = "添加失败，打印机已存在！";
                            return;
                        }
                    }
                    foreach(var item in printIP)
                    {
                        if (txt.Text == item.Name)
                        {
                            printName.Add(new AddPrintersMessage { Name = item.Name, IP = item.IP, ID = num.ToString()}) ;
                            num++;
                        }
                    }
                    txtAddPrinterCount.Text = printName.Count.ToString();
                }
            }
        }

        //从新添加的打印机中删除打印机
        private void btnSubPrinter_Click(object sender, RoutedEventArgs e)
        {
            printer = new Printer();
            List<Button> button = printer.GetChildObjects<Button>(itemPrinters2, "");
            foreach (Button btn in button)
            {
                if (btn.IsFocused)
                {
                    DependencyObject dependency = VisualTreeHelper.GetParent(btn);                    
                    TextBlock txt = printer.FindFirstVisualChild<TextBlock>(dependency, "txtPrinterIP");
                    foreach (var item in printName)
                    {
                        if (txt.Text == item.IP)
                        {
                            printName.Remove(item);
                            txtAddPrinterCount.Text = printName.Count.ToString();
                            return;
                        }
                    }
                    
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (printName != null)
                {
                    StreamWriter streamWrite = new StreamWriter(FileTools.executePrintersFilePath);
                    foreach (var item in printName)
                    {
                        streamWrite.Write(item.Name + ";" + item.IP + ";" + "\r\n");//写入数据
                    }                   
                    streamWrite.Close();//关闭文件
                }
                else
                {
                    txtMessage.Text = "没有添加内容";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        
    }
}
