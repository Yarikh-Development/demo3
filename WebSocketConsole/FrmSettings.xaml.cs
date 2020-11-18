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
using System.IO;
using WebSocketConsole.Properties;

namespace WebSocketConsole
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class FrmSettings : Window
    {
        public FrmSettings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String sErr = "";
            try
            {
                this.Cursor = Cursors.Wait;

                txtIPs.Text = My_Functions.GetIP_EX(ref sErr);
                if (sErr != "")
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                }
                this.txtPort.Text = Settings.Default.Port.ToString();
                this.txtCertificateFile.Text = Settings.Default.CertificateFile;
                this.txtCertificatePassword.Password = My_Functions.DecryptDES(Settings.Default.CertificatePassword, My_Functions.encryptKey);
                this.txtAppPath.Text = AppDomain.CurrentDomain.BaseDirectory;
                this.txtAlias.Text = Settings.Default.Identification_Alias;
                //chkHTTPS.Checked = true;
                if (Settings.Default.HTTPS)
                {
                    chkHTTPS.IsChecked = true;
                }
                else
                {
                    chkHTTPS.IsChecked = false;
                }
                chkHTTPS_Checked(sender, e);
                txtSQLSeverName.Text = Settings.Default.SQL_ServerName;
                txtSQLPort.Text = Settings.Default.SQL_Port.ToString();
                txtSQLTimeout.Text = Settings.Default.SQL_TimeOut.ToString();
                txtSQLUserName.Text = Settings.Default.SQL_UserName;
                txtSQLPassword.Password = My_Functions.DecryptDES(Settings.Default.SQL_Password, My_Functions.encryptKey);

                txtSQLDatabaseName.Text = Settings.Default.SQL_DatabaseName;
                // chkPrintWelcome.Checked= Settings.Default.PrintWelcomeLabel ;

                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void cmdSaveSetting_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPort.Text == "")
                {
                    MessageBox.Show("Please input Listen Port!", this.Title, MessageBoxButton.OK);
                    txtPort.Focus();
                    return;
                }

                if (txtAlias.Text == "")
                {
                    MessageBox.Show("Please input Identification/Alias!", this.Title, MessageBoxButton.OK);
                    txtAlias.Focus();
                    return;
                }

                //1118暂时注释
                /*if (txtCertificateFile.Text == "")
                {
                    MessageBox.Show("Please input Certificate File Name!", this.Title, MessageBoxButton.OK);
                    txtCertificateFile.Focus();
                    return;
                }*/

                if (txtCertificateFile.Text != "")
                {
                    if (!System.IO.File.Exists(System.IO.Path.Combine(txtAppPath.Text, txtCertificateFile.Text)))
                    {
                        MessageBox.Show("Certificate File does not exist!", this.Title, MessageBoxButton.OK);
                        txtCertificateFile.Focus();
                        return;
                    }
                }


                //1118暂时注释
                /*if (txtCertificatePassword.Password == "")
                {
                    MessageBox.Show("Please input Certificate Password!", this.Title, MessageBoxButton.OK);
                    txtCertificatePassword.Focus();
                    return;
                }*/

                if (txtSQLSeverName.Text == "")
                {
                    MessageBox.Show("Please input Server Name/IP!", this.Title, MessageBoxButton.OK);
                    txtSQLSeverName.Focus();
                    return;
                }
                if (txtSQLPort.Text == "")
                {
                    MessageBox.Show("Please input Server Port!", this.Title, MessageBoxButton.OK);
                    txtSQLPort.Focus();
                    return;
                }
                if (txtSQLTimeout.Text == "")
                {
                    MessageBox.Show("Please input request timeout interval!", this.Title, MessageBoxButton.OK);
                    txtSQLTimeout.Focus();
                    return;
                }

                this.Cursor = Cursors.Wait;
                if (txtPort.Text == "")
                {
                    txtPort.Text = "443";
                }

                Settings.Default.Port = int.Parse(txtPort.Text);
                Settings.Default.Identification_Alias = txtAlias.Text;
                //Settings.Default.CertificateFile = txtCertificateFile.Text;
                //Settings.Default.CertificatePassword = My_Functions.EncryptDES(txtCertificatePassword.Password, My_Functions.encryptKey);
                //1118暂时上线
                if (txtCertificateFile.Text == "")
                {
                    Settings.Default.CertificateFile = "yserver.yarikh.cn.p12";
                }
                if (txtCertificatePassword.Password == "")
                {
                    Settings.Default.CertificatePassword = My_Functions.EncryptDES("yarikh2019", My_Functions.encryptKey);
                }
                
                Settings.Default.HTTPS = (bool)chkHTTPS.IsChecked;
                Settings.Default.SQL_ServerName = txtSQLSeverName.Text;
                Settings.Default.DB_Connection_Type = 1;
                Settings.Default.SQL_Authentication_Type = 1;
                
                if (txtSQLPort.Text == "")
                {
                    txtSQLPort.Text = "1433";
                }
                if (txtSQLTimeout.Text == "")
                {
                    txtSQLTimeout.Text = "60";
                }
                Settings.Default.SQL_Port = int.Parse(txtSQLPort.Text);
                Settings.Default.SQL_TimeOut = int.Parse(txtSQLTimeout.Text);
                Settings.Default.SQL_UserName = txtSQLUserName.Text;
                Settings.Default.SQL_Password = My_Functions.EncryptDES(txtSQLPassword.Password, My_Functions.encryptKey);               
                Settings.Default.SQL_DatabaseName = txtSQLDatabaseName.Text;
                // Settings.Default.PrintWelcomeLabel = chkPrintWelcome.Checked;
                Properties.Settings.Default.Save();
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("The settings were successfully saved!", this.Title, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void chkHTTPS_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkHTTPS.IsChecked)
                {
                    txtCertificateFile.IsEnabled = true;
                    txtCertificatePassword.IsEnabled = true;
                }
                else
                {
                    txtCertificateFile.IsEnabled = false;
                    txtCertificatePassword.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }        

        private void txtCertificatePassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLinkSetting_Click(object sender, RoutedEventArgs e)
        {
            gridLinkSetting.Visibility = Visibility.Visible;
            gridDataSetting.Visibility = Visibility.Hidden;
        }

        private void btnBaseSetting_Click(object sender, RoutedEventArgs e)
        {
            gridLinkSetting.Visibility = Visibility.Hidden;
            gridDataSetting.Visibility = Visibility.Visible;
        }

        private void btnOthersSetting_Click(object sender, RoutedEventArgs e)
        {
            gridLinkSetting.Visibility = Visibility.Hidden;
            gridDataSetting.Visibility = Visibility.Hidden;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnMiniWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
