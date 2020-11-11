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

                /*if (Settings.Default.DB_Connection_Type == 0)
                {
                    optWebservice.Checked = true;
                    optSQLClient.Checked = false;
                }
                else
                {
                    optWebservice.Checked = false;
                    optSQLClient.Checked = true;
                }*/
                // OptWebservice_CheckedChanged(sender, e);

                // txtWebServiceURL.Text = Settings.Default.WebServiceURL;
                /*if (Settings.Default.EnabledProxy)
                {
                    chkProxy.Checked = true;
                }
                else
                {
                    chkProxy.Checked = false;
                }*/
                /*ChkProxy_CheckedChanged(sender, e);
                txtProxyName.Text = Settings.Default.ProxyName;
                txtProxyPort.Text = Settings.Default.ProxyPort.ToString();
                if (Settings.Default.VerifyProxyUserNamePassword)
                {
                    chkVerifyProxyUserPassword.Checked = true;
                }
                else
                {
                    chkVerifyProxyUserPassword.Checked = false;
                }
                // ChkVerifyProxyUserPassword_CheckedChanged(sender, e);
                txtProxyUserName.Text = Settings.Default.ProxyUserName;
                txtProxyPassword.Text = My_Functions.DecryptDES(Settings.Default.ProxyPassword, My_Functions.encryptKey);

                if (Settings.Default.DB_Connection_Type == 0)
                {
                    optWA.Checked = true;
                    optSA.Checked = false;
                }
                else
                {
                    optWA.Checked = false;
                    optSA.Checked = true;
                }*/
                // OptWA_CheckedChanged(sender, e);
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

        /*private void FrmSettings_Load(object sender, EventArgs e)
        {
            
        }*/
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

                if (txtCertificateFile.Text == "")
                {
                    MessageBox.Show("Please input Certificate File Name!", this.Title, MessageBoxButton.OK);
                    txtCertificateFile.Focus();
                    return;
                }

                if (txtCertificateFile.Text != "")
                {
                    if (!System.IO.File.Exists(System.IO.Path.Combine(txtAppPath.Text, txtCertificateFile.Text)))
                    {
                        MessageBox.Show("Certificate File does not exist!", this.Title, MessageBoxButton.OK);
                        txtCertificateFile.Focus();
                        return;
                    }
                }

                if (txtCertificatePassword.Password == "")
                {
                    MessageBox.Show("Please input Certificate Password!", this.Title, MessageBoxButton.OK);
                    txtCertificatePassword.Focus();
                    return;
                }

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

                /*if (optWebservice.Checked)
                {
                    if (txtWebServiceURL.Text == "")
                    {
                        MessageBox.Show("Please input WebService URL!", (string)this.Content, MessageBoxButton.OK);
                        txtWebServiceURL.Focus();
                        return;
                    }
                }*/

                /*if (optSQLClient.Checked)
                {
                    if (txtSQLSeverName.Text == "")
                    {
                        MessageBox.Show("Please input Server Name/IP!", (string)this.Content, MessageBoxButton.OK);
                        txtSQLSeverName.Focus();
                        return;
                    }
                    if (txtSQLPort.Text == "")
                    {
                        MessageBox.Show("Please input Server Port!", (string)this.Content, MessageBoxButton.OK);
                        txtSQLPort.Focus();
                        return;
                    }
                    if (txtSQLTimeout.Text == "")
                    {
                        MessageBox.Show("Please input request timeout interval!", (string)this.Content, MessageBoxButton.OK);
                        txtSQLTimeout.Focus();
                        return;
                    }
                }*/

                this.Cursor = Cursors.Wait;
                if (txtPort.Text == "")
                {
                    txtPort.Text = "443";
                }

                Settings.Default.Port = int.Parse(txtPort.Text);
                Settings.Default.Identification_Alias = txtAlias.Text;
                Settings.Default.CertificateFile = txtCertificateFile.Text;
                Settings.Default.CertificatePassword = My_Functions.EncryptDES(txtCertificatePassword.Password, My_Functions.encryptKey);
                Settings.Default.HTTPS = (bool)chkHTTPS.IsChecked;
                Settings.Default.SQL_ServerName = txtSQLSeverName.Text;
                Settings.Default.DB_Connection_Type = 1;
                Settings.Default.SQL_Authentication_Type = 1;
                /*if (optWebservice.Checked)
                {
                    Settings.Default.DB_Connection_Type = 0;
                }
                if (optSQLClient.Checked)
                {
                    Settings.Default.DB_Connection_Type = 1;
                }
                Settings.Default.SQL_ServerName = txtSQLSeverName.Text;
                if (optWA.Checked)
                {
                    Settings.Default.SQL_Authentication_Type = 0;
                }
                else
                {
                    Settings.Default.SQL_Authentication_Type = 1;
                }*/
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
                // Settings.Default.WebServiceURL = txtWebServiceURL.Text;
                /*if (chkProxy.Checked)
                {
                    Settings.Default.EnabledProxy = true;
                }
                else
                {
                    Settings.Default.EnabledProxy = false;
                }*/
                // Settings.Default.ProxyName = txtProxyName.Text;
                /*if (txtProxyPort.Text == "")
                {
                    txtProxyPort.Text = "8080";
                }*/

                // Settings.Default.ProxyPort = int.Parse(txtProxyPort.Text);
                /*if (chkVerifyProxyUserPassword.Checked)
                {
                    Settings.Default.VerifyProxyUserNamePassword = true;
                }
                else
                {
                    Settings.Default.VerifyProxyUserNamePassword = false;
                }*/
                // Settings.Default.ProxyUserName = txtProxyUserName.Text;
                // Settings.Default.ProxyPassword = txtProxyPassword.Text;

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

/*        private void ChkHTTPS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHTTPS.Checked)
                {
                    txtCertificateFile.Enabled = true;
                    txtCertificatePassword.Enabled = true;
                }
                else
                {
                    txtCertificateFile.Enabled = false;
                    txtCertificatePassword.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

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

        /*private void OptWebservice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optWebservice.Checked)
                {
                    pnlWebService.Visible = true;
                    pnlWebService.BringToFront();
                    pnlSQLClient.Visible = false;
                }
                else
                {
                    pnlWebService.Visible = false;
                    pnlSQLClient.Visible = true;
                    pnlSQLClient.BringToFront();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        /*private void OptWA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optWA.Checked)
                {
                    lblSLQUserPassword.Enabled = false;
                    lblDatabaseName.Enabled = false;
                    txtSQLUserName.Enabled = false;
                    txtSQLPassword.Enabled = false;
                    txtSQLDatabaseName.Enabled = false;

                }
                else
                {
                    lblSLQUserPassword.Enabled = true;
                    lblDatabaseName.Enabled = true;
                    txtSQLUserName.Enabled = true;
                    txtSQLPassword.Enabled = true;
                    txtSQLDatabaseName.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        /*private void OptSQLClient_CheckedChanged(object sender, EventArgs e)
        {
            OptWebservice_CheckedChanged(sender, e);
        }

        private void OptSA_CheckedChanged(object sender, EventArgs e)
        {
            OptWA_CheckedChanged(sender, e);
        }*/

        /* private void ChkProxy_CheckedChanged(object sender, EventArgs e)
         {
             try
             {
                 if (chkProxy.Checked)
                 {
                     lblProxyServer.Enabled = true;
                     txtProxyName.Enabled = true;
                     txtProxyPort.Enabled = true;
                     chkVerifyProxyUserPassword.Enabled = true;
                 }
                 else
                 {
                     lblProxyServer.Enabled = false;
                     txtProxyName.Enabled = false;
                     txtProxyPort.Enabled = false;
                     chkVerifyProxyUserPassword.Enabled = false;
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
         }*/

        /*private void ChkVerifyProxyUserPassword_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVerifyProxyUserPassword.Checked)
                {
                    lblProxyUserPassword.Enabled = true;
                    txtProxyUserName.Enabled = true;
                    txtProxyPassword.Enabled = true;
                }
                else
                {
                    lblProxyUserPassword.Enabled = false;
                    txtProxyUserName.Enabled = false;
                    txtProxyPassword.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        private void txtCertificatePassword_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
