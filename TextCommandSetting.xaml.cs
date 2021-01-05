using demo.Properties;
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
using System.Windows.Threading;

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
        private int timeMode = 0;
        public TextCommandSetting()
        {
            InitializeComponent();
            
        }

        public TextCommandSetting(string buttonName)
        {
            InitializeComponent();
            RadioButton radioButton = panelMenu.FindName(buttonName) as RadioButton;
            radioButton.IsChecked = true;
            
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Timer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
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
            if (btnOdometerAdmin.IsChecked == true)
            {
                gdOdometerAdmin.Visibility = Visibility.Visible;
            }
            dtpAnyTime.SelectedDateTime = Settings.Default.anyTime;
            dtpDays.SelectedDateTime = Settings.Default.days;
            dtpHours.SelectedDateTime = Settings.Default.hours;
            dtpMinutes.SelectedDateTime = Settings.Default.minutes;
            cbIsDays.IsChecked = Settings.Default.isDays;
            cbIsHours.IsChecked = Settings.Default.isHours;
            cbIsMinutes.IsChecked = Settings.Default.isMinutes;
        }

        private void Timer(object sender, EventArgs e)
        {
            if (cbIsDays.IsChecked == true && cbIsHours.IsChecked == true && cbIsMinutes.IsChecked == true)
            {
                //1
                timeMode = 1;
                dtpAnyTime.IsEnabled = false;
                cbbSelectDaysMode.IsEnabled = true;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = true;
            }
            else if (cbIsDays.IsChecked == true && cbIsHours.IsChecked == true && cbIsMinutes.IsChecked == false)
            {
                //2
                timeMode = 2;
                dtpAnyTime.IsEnabled = false;
                cbbSelectDaysMode.IsEnabled = true;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = true;
                dtpMinutes.IsEnabled = false;
            }
            else if (cbIsDays.IsChecked == true && cbIsHours.IsChecked == false && cbIsMinutes.IsChecked == true)
            {
                //3
                timeMode = 3;
                dtpAnyTime.IsEnabled = false;
                cbbSelectDaysMode.IsEnabled = true;
                dtpDays.IsEnabled = true;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = true;
            }
            else if (cbIsDays.IsChecked == true && cbIsHours.IsChecked == false && cbIsMinutes.IsChecked == false)
            {
                //4
                timeMode = 4;
                dtpAnyTime.IsEnabled = false;
                cbbSelectDaysMode.IsEnabled = true;
                dtpDays.IsEnabled = true;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = false;
            }
            else if (cbIsDays.IsChecked == false && cbIsHours.IsChecked == true && cbIsMinutes.IsChecked == true)
            {
                //5
                timeMode = 5;
                dtpAnyTime.IsEnabled = true;
                cbbSelectDaysMode.IsEnabled = false;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = true;
            }
            else if (cbIsDays.IsChecked == false && cbIsHours.IsChecked == true && cbIsMinutes.IsChecked == false)
            {
                //6
                timeMode = 6;
                dtpAnyTime.IsEnabled = true;
                cbbSelectDaysMode.IsEnabled = false;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = true;
                dtpMinutes.IsEnabled = false;
            }
            else if (cbIsDays.IsChecked == false && cbIsHours.IsChecked == false && cbIsMinutes.IsChecked == true)
            {
                //7
                timeMode = 7;
                dtpAnyTime.IsEnabled = true;
                cbbSelectDaysMode.IsEnabled = false;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = true;
            }
            else if (cbIsDays.IsChecked == false && cbIsHours.IsChecked == false && cbIsMinutes.IsChecked == false)
            {
                //8
                timeMode = 8;
                dtpAnyTime.IsEnabled = true;
                cbbSelectDaysMode.IsEnabled = false;
                dtpDays.IsEnabled = false;
                dtpHours.IsEnabled = false;
                dtpMinutes.IsEnabled = false;
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
            gdOdometerAdmin.Visibility = Visibility.Hidden;
        }

        private void btnOdometerAdmin_Click(object sender, RoutedEventArgs e)
        {
            gdEditCommand.Visibility = Visibility.Hidden;
            gdOdometerAdmin.Visibility = Visibility.Visible;
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

        private void cbIsDays_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbIsHours_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbIsMinutes_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbbSelectDaysMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                if (cbbSelectDaysMode.SelectedIndex == 1)
                {
                    panelSelectDays1.IsEnabled = true;
                    panelSelectDays2.IsEnabled = true;
                }
                else
                {
                    panelSelectDays1.IsEnabled = false;
                    panelSelectDays2.IsEnabled = false;
                }
            }
        }

        private void btnSaveOdometer_Click(object sender, RoutedEventArgs e)
        {
            if (timeMode == 1)
            {
                string seconds = dtpMinutes.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} * * * * ?";
                Settings.Default.anyTime = DateTime.Today;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = dtpMinutes.SelectedDateTime;
                Settings.Default.isDays = true;
                Settings.Default.isHours = true;
                Settings.Default.isMinutes = true;
            }
            else if (timeMode == 2)
            {
                string hours = dtpHours.SelectedDateTime.ToString("mm");
                string minutes = dtpHours.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{minutes} {hours} * * * ?";
                Settings.Default.anyTime = DateTime.Today;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = dtpHours.SelectedDateTime;
                Settings.Default.minutes = DateTime.Today;
                Settings.Default.isDays = true;
                Settings.Default.isHours = true;
                Settings.Default.isMinutes = false;
            }
            else if (timeMode == 3)
            {
                string hours = dtpDays.SelectedDateTime.ToString("HH");
                string seconds = dtpMinutes.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} * {hours} * * ?";
                Settings.Default.anyTime = DateTime.Today;
                Settings.Default.days = dtpDays.SelectedDateTime;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = dtpMinutes.SelectedDateTime;
                Settings.Default.isDays = true;
                Settings.Default.isHours = false;
                Settings.Default.isMinutes = true;
            }
            else if (timeMode == 4)
            {
                string hours = dtpDays.SelectedDateTime.ToString("HH");
                string minutes = dtpDays.SelectedDateTime.ToString("mm");
                string seconds = dtpDays.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} {minutes} {hours} * * ?";
                Settings.Default.anyTime = DateTime.Today;
                Settings.Default.days = dtpDays.SelectedDateTime;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = DateTime.Today;
                Settings.Default.isDays = true;
                Settings.Default.isHours = false;
                Settings.Default.isMinutes = false;
            }
            else if (timeMode == 5)
            {
                string year = dtpAnyTime.SelectedDateTime.ToString("yyyy");
                string month = dtpAnyTime.SelectedDateTime.ToString("MM");
                string day = dtpAnyTime.SelectedDateTime.ToString("dd");
                string seconds = dtpMinutes.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} * * {day} {month} ? {year}";
                Settings.Default.anyTime = dtpAnyTime.SelectedDateTime;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = dtpMinutes.SelectedDateTime;
                Settings.Default.isDays = false;
                Settings.Default.isHours = true;
                Settings.Default.isMinutes = true;
            }
            else if (timeMode == 6)
            {
                string year = dtpAnyTime.SelectedDateTime.ToString("yyyy");
                string month = dtpAnyTime.SelectedDateTime.ToString("MM");
                string day = dtpAnyTime.SelectedDateTime.ToString("dd");
                string minutes = dtpHours.SelectedDateTime.ToString("mm");
                string seconds = dtpHours.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} {minutes} * {day} {month} ? {year}";
                Settings.Default.anyTime = dtpAnyTime.SelectedDateTime;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = dtpHours.SelectedDateTime;
                Settings.Default.minutes = DateTime.Today;
                Settings.Default.isDays = false;
                Settings.Default.isHours = true;
                Settings.Default.isMinutes = false;
            }
            else if (timeMode == 7)
            {
                string year = dtpAnyTime.SelectedDateTime.ToString("yyyy");
                string month = dtpAnyTime.SelectedDateTime.ToString("MM");
                string day = dtpAnyTime.SelectedDateTime.ToString("dd");
                string hours = dtpAnyTime.SelectedDateTime.ToString("HH");
                string seconds = dtpMinutes.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} * {hours} {day} {month} ? {year}";
                Settings.Default.anyTime = dtpAnyTime.SelectedDateTime;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = dtpMinutes.SelectedDateTime;
                Settings.Default.isDays = false;
                Settings.Default.isHours = false;
                Settings.Default.isMinutes = true;
            }
            else if (timeMode == 8)
            {
                string year = dtpAnyTime.SelectedDateTime.ToString("yyyy");
                string month = dtpAnyTime.SelectedDateTime.ToString("MM");
                string day = dtpAnyTime.SelectedDateTime.ToString("dd");
                string hours = dtpAnyTime.SelectedDateTime.ToString("HH");
                string minutes = dtpAnyTime.SelectedDateTime.ToString("mm");
                string seconds = dtpAnyTime.SelectedDateTime.ToString("ss");
                Settings.Default.odoAdminTime = $"{seconds} {minutes} {hours} {day} {month} ? {year}";
                Settings.Default.anyTime = dtpAnyTime.SelectedDateTime;
                Settings.Default.days = DateTime.Today;
                Settings.Default.hours = DateTime.Today;
                Settings.Default.minutes = DateTime.Today;
                Settings.Default.isDays = false;
                Settings.Default.isHours = false;
                Settings.Default.isMinutes = false;
            }
            Settings.Default.Save();
        }
    }
}
