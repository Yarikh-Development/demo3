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

namespace demo
{
    /// <summary>
    /// Printer.xaml 的交互逻辑
    /// </summary>
    public partial class PrinterWindow : Window
    {
        public PrinterWindow()
        {
        }

        public PrinterWindow(Window window)
        {
            InitializeComponent();
            CreateTabControlPage("设置", new PrinterSettingPage(), Printer_TabControl);
            CreateTabControlPage("信息", new PrinterInfoPage(), Printer_TabControl);
            this.Owner = window;
        }

        private void CreateTabControlPage(string pageName, Page page, TabControl tabControl)
        {
            if (pageName == string.Empty || page == null || tabControl == null)
                return;
            TabItem tabitem = new TabItem();
            tabitem.Header = pageName;
            Frame tabFrame = new Frame();
            tabFrame.Content = page;
            tabitem.Content = tabFrame;
            tabControl.Items.Add(tabitem);
        }
    }
}
