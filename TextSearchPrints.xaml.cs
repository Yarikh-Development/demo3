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

        private void printerMessage_Click(object sender, RoutedEventArgs e)
        {
            isVisibilityForEnter.Visibility = Visibility.Visible;
            //TextBlock txt = FindFirstVisualChild<TextBlock>(itemsPrinters, "txtPrintersName");
            //txtPrinterName2.Text = txt.Text;
            List<TextBlock> collection = GetChildObjects<TextBlock>(itemsPrinters, "");//第2个参数为空，表示查找所有指定类型的控件（返回
            List<Button> buttonItems = GetChildObjects<Button>(itemsPrinters, "");
            
            foreach (Button button in buttonItems)
            {                
                if (button.IsFocused)
                {
                    TextBlock txt = FindFirstVisualChild<TextBlock>(button, "txtPrintersName");
                    //MessageBox.Show(txt.Text);
                    txtPrinterName2.Text = txt.Text;
                }
                    //MessageBox.Show(button.Name);                    
            }
            /*foreach (TextBlock item in collection) //遍历这个集合
            {
                if (item)
                    MessageBox.Show(item.Text.ToString() + "被选中了!");
            }*/
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        /*private void txtPrintersName_TouchDown(object sender, TouchEventArgs e)
        {
            if (itemsPrinters.SelectedIndex < 0) return;

            ListBoxItem myListBoxItem = (ListBoxItem)(NewVideoListBox.ItemContainerGenerator.ContainerFromIndex(NewVideoListBox.SelectedIndex));
            ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            MaterialDesignThemes.Wpf.Badged badged = (MaterialDesignThemes.Wpf.Badged)myDataTemplate.FindName("CountingBadge", myContentPresenter);

            TextBlock likeText = (TextBlock)myDataTemplate.FindName("LikeText", myContentPresenter);

            likeText.Foreground = new SolidColorBrush(Colors.Red);



            if (badged.Badge == null)
                badged.Badge = 0;
            int i;
            int.TryParse(badged.Badge.ToString(), out i);
            badged.Badge = i + 1;
        }*/

        public T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject

        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, ""));//指定集合的元素添加到List队尾
            }
            return childList;
        }
    }
}
