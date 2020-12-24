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
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// TextCommandSetting.xaml 的交互逻辑
    /// </summary>
    public partial class TextCommandSetting : Window
    {

        private string direct = "";
        private string explain = "";
        private int line = -1;
        public TextCommandSetting()
        {
            InitializeComponent();
        }

        public TextCommandSetting(string direct, string explain, string detailExplain, int line, string flag)
        {
            InitializeComponent();
            this.direct = direct;
            this.explain = explain;
            this.line = line;
            txtCommand.Text = direct;
            txtCommandTitle.Text = explain;
            txtCommandExplain.Text = detailExplain;
            if (flag == "ZPL")
            {
                btnSelectSGD.IsEnabled = false;
            }
            else
            {
                btnSelectSGD.IsChecked = true;
                btnSelectZPL.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (btnEdit.IsChecked == true)
            {
                gdEditCommand.Visibility = Visibility.Visible;
            }
        }

        private void btnSaveCommand_Click(object sender, RoutedEventArgs e)
        {
            txtLogMessage.Foreground = new SolidColorBrush(Colors.Red);
            if (txtCommand.Text == "" || txtCommandTitle.Text == "")
            {
                txtLogMessage.Text = "内容为空，请正确输入！";
                return;
            }
            if (txtCommandExplain.Text == "")
            {
                txtCommandExplain.Text = txtCommandTitle.Text;
            }
            string data = txtCommand.Text + ';' + txtCommandTitle.Text + ';' + txtCommandExplain.Text + ';';
            if (line == -1)
            {
                if (btnSelectSGD.IsChecked == true)
                {                                               
                    FileTools.WriteLineFile(FileTools.commandPath + "\\commandSGD.txt", data);                        
                }
                if (btnSelectZPL.IsChecked == true)
                {
                    FileTools.WriteLineFile(FileTools.commandPath + "\\commandZPL.txt", data);                        
                }
            }
            else if (line >= 0)
            {
                if (btnSelectSGD.IsChecked == true)
                {                      
                    FileTools.CoverLine(FileTools.commandPath + "\\commandSGD.txt", data, line);
                }
                if (btnSelectZPL.IsChecked == true)
                {                        
                    FileTools.CoverLine(FileTools.commandPath + "\\commandZPL.txt", data, line);
                }
            }
            if (line >= 0)
                this.Close();
            txtLogMessage.Text = "保存成功！";
            if (txtLogMessage.Text == "保存成功！")
                txtLogMessage.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtCommand.Text = "";
            txtCommandTitle.Text = "";
            txtCommandExplain.Text = "";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            gdEditCommand.Visibility = Visibility.Visible;

        }

        private void btnOdometerAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMiniWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
