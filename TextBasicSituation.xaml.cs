using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextBasicSituation.xaml 的交互逻辑
    /// </summary>
    public partial class TextBasicSituation : Page
    {
        public TextBasicSituation()
        {
            InitializeComponent();
        }

        private void btnBaseMessage_Click(object sender, RoutedEventArgs e)
        {
            svBaseMessage.Visibility = Visibility.Visible;
            gdOdometer.Visibility = Visibility.Hidden;
        }

        private void btnPrintOdometer_Click(object sender, RoutedEventArgs e)
        {
            svBaseMessage.Visibility = Visibility.Hidden;
            gdOdometer.Visibility = Visibility.Visible;
        }

        private void cbTimesSelect_Click(object sender, RoutedEventArgs e)
        {
            if (cbTimesSelect.IsChecked == true)
            {
                dpTimesSelect1.IsEnabled = true;
                dpTimesSelect2.IsEnabled = true;
            }
            else
            {
                dpTimesSelect1.IsEnabled = false;
                dpTimesSelect2.IsEnabled = false;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (cbTimesSelect.IsChecked == false)
            {
                if (dpTimeSelect.SelectedDate != null)
                {
                    lvTimesOdometer.ItemsSource =  LinkOSPrinters.CalcOdometer((DateTime)dpTimeSelect.SelectedDate);

                }
                else
                {
                    MessageBox.Show("请正确输入日期");
                }
            }
            else
            {
                if (dpTimesSelect1.SelectedDate != null && dpTimesSelect2.SelectedDate != null)
                {
                    //此段注释不要删除，判断如果选择的时间没找到文件时的提示
                    /*string filePath1 = FileTools.odometerLogPath + "\\" + 
                        ((DateTime)dpTimesSelect1.SelectedDate).ToString("yyyy-MM-dd") + ".txt";
                    string filePath2 = FileTools.odometerLogPath + "\\" +
                        ((DateTime)dpTimesSelect2.SelectedDate).ToString("yyyy-MM-dd") + ".txt";
                    if (!File.Exists(filePath1))
                    {
                        MessageBox.Show($"没有找到{dpTimesSelect1.SelectedDate}的日志！");
                    }
                    if (!File.Exists(filePath2))
                    {
                        MessageBox.Show($"没有找到{dpTimesSelect2.SelectedDate}的日志！");
                    }*/
                    lvTimesOdometer.ItemsSource = LinkOSPrinters.CalcOdometer((DateTime)dpTimesSelect1.SelectedDate, (DateTime)dpTimesSelect2.SelectedDate);

                }
                else
                {
                    MessageBox.Show("请正确输入日期");
                }
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            TextCommandSetting commandSetting = new TextCommandSetting("btnOdometerAdmin");
            commandSetting.ShowDialog();
        }
    }
}
