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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Drawing;
//using System.Windows.Forms;
using System.Diagnostics;
using WebSocketsCmd.Client;
using WebSocketConsole.Properties;
using WebSockets.Common;
using System.Threading;
using WebSockets;
using WebSockets.Events;
using WebSocketsCmd.Server;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WebSocketsCmd;
using Newtonsoft.Json.Linq;

namespace WebSocketConsole
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int TotalDevices = 0;
        private static IWebSocketLogger _logger;
        ServiceFactory serviceFactory = null;
        WebServer server = null;
        COMM.BusinessObject My_CommObject = null;
        String gsCertificateKEY = "96FD10C974A864196E3FD7F2C71BAB";
        //String APPKEY = "ES";

        String gsSerialNumber = "";
        String gsCurrentIP = "";
        String gsLocalMachineName = "";
        String gsPlatForm = "";

        int port = 0;
        String CertificateFile = "";
        String CertificatePassword = "";
        String webRoot = "";
        String Identification_Alias = "";
        int mnCounter = 0;
        public String msCurrentSelectedDeviceSN = "";
        private static String msFMT_Test_Label = "";

        public String RefreshGrid(String sConnection_Info)
        {
            String sTemp = "";
            int nTotal = 0;
            try
            {
                listView1.Items.Clear();
                DataTable sample = new DataTable(); //Sample Data
                sample.Columns.Add("LineNo", typeof(string));
                sample.Columns.Add("PrinterName", typeof(string));
                sample.Columns.Add("SerialNumber", typeof(string));
                sample.Columns.Add("FWVersion", typeof(string));
                sample.Columns.Add("Location", typeof(string));
                sample.Columns.Add("IP", typeof(string));
                sample.Columns.Add("TimeStamp", typeof(string));
                sample.Columns.Add("Alert", typeof(string));


                nTotal = System.Text.RegularExpressions.Regex.Matches(sConnection_Info, Environment.NewLine).Count;
                for (int i = 0; i <= nTotal - 1; i++)
                {
                    sTemp = sConnection_Info.Split('\n')[i];
                    DataRow row = sample.NewRow();
                    row["LineNo"] = i + 1;
                    row["PrinterName"] = sTemp.Split('|')[2];
                    row["SerialNumber"] = sTemp.Split('|')[1];
                    row["FWVersion"] = sTemp.Split('|')[3];
                    row["Location"] = sTemp.Split('|')[4];
                    row["IP"] = sTemp.Split('|')[5];
                    row["Alert"] = sTemp.Split('|')[7];
                    sample.Rows.Add(row);
                }
                sample.AcceptChanges();
                sample.Dispose();
                return sTemp;
            }
            catch (Exception ex)
            {
                sTemp = ex.Message;
                return sTemp;
            }
        }

        private static X509Certificate2 GetCertificate(string certFile, string certPassword, ref String sErr)
        {
            sErr = "";
            // it is clearly WRONG to store the certificate and password insecurely on disk like this but this is a demo
            // you would normally use the built in windows certificate store
            if (!File.Exists(certFile))
            {
                sErr = "Certificate file not found: " + certFile;
            }
            var cert = new X509Certificate2(certFile, certPassword);
            _logger.Information("", typeof(MainWindow), "Successfully loaded certificate");
            return cert;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            try
            {
                // listView1.Columns[listView1.Columns.Count - 1].Width = listView1.Width - 1150 - 30;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "", MessageBoxButton.OK);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            try
            {
                this.Title = this.Title + "   Ver:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                port = Settings.Default.Port;
                CertificateFile = Settings.Default.CertificateFile;
                CertificatePassword = Settings.Default.CertificatePassword;
                CertificatePassword = My_Functions.DecryptDES(CertificatePassword, My_Functions.encryptKey);
                Identification_Alias = Settings.Default.Identification_Alias;

                picOrange.Visibility = Visibility.Visible;
                picGreen.Visibility = Visibility.Hidden;

                FrmMain_Resize(sender, e);
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void cmdStart_Service_Click(object sender, EventArgs e)
        {
            {
                _logger = new WebSocketLogger();
                String sErr = "";

                Core_Lib.Utility Utility = new Core_Lib.Utility();
                gsSerialNumber = Utility.GetSerialNumber(ref sErr);
                if (sErr != "")
                {
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                    cmdStart_Service.Focus();
                    return;
                }

                gsCurrentIP = Utility.GetCurrentIP(ref sErr);
                if (sErr != "")
                {
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                    cmdStart_Service.Focus();
                    return;
                }

                gsLocalMachineName = Utility.GetLocalMachineName(ref sErr);
                if (sErr != "")
                {
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                    cmdStart_Service.Focus();
                    return;
                }

                gsPlatForm = Utility.GetPlatFormType(ref sErr);
                if (sErr != "")
                {
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                    cmdStart_Service.Focus();
                    return;
                }

                try
                {
                    webRoot = AppDomain.CurrentDomain.BaseDirectory; ;

                    if (!System.IO.File.Exists(webRoot + Settings.Default.CertificateFile))
                    {
                        MessageBox.Show("Certificate File does not exist!", this.Title, MessageBoxButton.OK);
                        return;
                    }

                    if (!Directory.Exists(webRoot))
                    {
                        string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                        _logger.Warning("", typeof(MainWindow), "Webroot folder {0} not found. Using application base directory: {1}", webRoot, baseFolder);
                        webRoot = baseFolder;
                    }

                    this.Cursor = Cursors.Wait;

                    // conect Database
                    My_CommObject = new COMM.BusinessObject();
                    My_CommObject.ApplicationName = this.Title;
                    if (Settings.Default.DB_Connection_Type == 0)
                    {
                        My_CommObject.Protocol = "HTTP";
                    }
                    else
                    {
                        My_CommObject.Protocol = "TCPIP";
                    }
                    My_CommObject.Server = Settings.Default.SQL_ServerName;
                    My_CommObject.Database = Settings.Default.SQL_DatabaseName;
                    if (Settings.Default.SQL_Authentication_Type == 0)
                    {
                        My_CommObject.IntegratedSecurity = true;
                    }
                    else
                    {
                        My_CommObject.IntegratedSecurity = false;
                    }
                    My_CommObject.Port = Settings.Default.SQL_Port;
                    My_CommObject.TimeOut = Settings.Default.SQL_TimeOut;
                    My_CommObject.UserID = Settings.Default.SQL_UserName;
                    My_CommObject.Password = My_Functions.DecryptDES(Settings.Default.SQL_Password, My_Functions.encryptKey);
                    My_CommObject.WebServiceURL = Settings.Default.WebServiceURL;
                    My_CommObject.ProxyServer = Settings.Default.ProxyName;
                    My_CommObject.ProxyPort = Settings.Default.ProxyPort;
                    My_CommObject.ProxyVerify = Settings.Default.VerifyProxyUserNamePassword;
                    My_CommObject.ProxyUser = Settings.Default.ProxyUserName;
                    My_CommObject.ProxyPassword = My_Functions.DecryptDES(Settings.Default.ProxyPassword, My_Functions.encryptKey);
                    My_CommObject.CertificateKEY = gsCertificateKEY;
                    My_CommObject.Connect(ref sErr); //开始连接
                    if (sErr != "")
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                        cmdStart_Service.Focus();
                        return;
                    }

                    // used to decide what to do with incoming connections
                    port = Settings.Default.Port;
                    CertificateFile = Settings.Default.CertificateFile;
                    CertificatePassword = Settings.Default.CertificatePassword;
                    CertificatePassword = My_Functions.DecryptDES(CertificatePassword, My_Functions.encryptKey);
                    Identification_Alias = Settings.Default.Identification_Alias;

                    serviceFactory = new ServiceFactory(webRoot, _logger, Identification_Alias, server, My_CommObject);
                    server = new WebServer(serviceFactory, _logger);
                    serviceFactory._server = server;
                    serviceFactory._My_CommObject = My_CommObject;
                    if (Settings.Default.HTTPS)
                    // if (port == 443)
                    {
                        X509Certificate2 cert = GetCertificate(webRoot + Settings.Default.CertificateFile, My_Functions.DecryptDES(Settings.Default.CertificatePassword, My_Functions.encryptKey), ref sErr);
                        server.Listen(port, cert);
                    }
                    else
                    {
                        server.Listen(port);
                    }

                    this.Cursor = Cursors.Arrow;
                    cmdStart_Service.IsEnabled = false;
                    cmdStop_Serivce.IsEnabled = true;
                    cmdStop_Serivce.Focus();
                    cmdViewLog.IsEnabled = true;
                    // timer1.Enabled = true;
                    cmdTest.IsEnabled = true;
                    // pnlSC.BackColor = Color.Blue;
                    picOrange.Visibility = Visibility.Hidden;
                    picGreen.Visibility = Visibility.Visible;
                    txtDevices.Text = "0";
                    txtSubscribers.Text = "0";
                    listView1.Items.Clear();
                    msCurrentSelectedDeviceSN = "";
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                }
            }
        }

        public void StartService(Button radioButton)
        {
            _logger = new WebSocketLogger();
            String sErr = "";

            Core_Lib.Utility Utility = new Core_Lib.Utility();
            gsSerialNumber = Utility.GetSerialNumber(ref sErr);
            if (sErr != "")
            {
                MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                radioButton.Focus();
                return;
            }

            gsCurrentIP = Utility.GetCurrentIP(ref sErr);
            if (sErr != "")
            {
                MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                radioButton.Focus();
                return;
            }

            gsLocalMachineName = Utility.GetLocalMachineName(ref sErr);
            if (sErr != "")
            {
                MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                radioButton.Focus();
                return;
            }

            gsPlatForm = Utility.GetPlatFormType(ref sErr);
            if (sErr != "")
            {
                MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                radioButton.Focus();
                return;
            }

            try
            {
                webRoot = AppDomain.CurrentDomain.BaseDirectory; ;

                if (!System.IO.File.Exists(webRoot + Settings.Default.CertificateFile))
                {
                    MessageBox.Show("Certificate File does not exist!", this.Title, MessageBoxButton.OK);
                    return;
                }

                if (!Directory.Exists(webRoot))
                {
                    string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                    _logger.Warning("", typeof(MainWindow), "Webroot folder {0} not found. Using application base directory: {1}", webRoot, baseFolder);
                    webRoot = baseFolder;
                }

                this.Cursor = Cursors.Wait;

                // conect Database
                My_CommObject = new COMM.BusinessObject();
                My_CommObject.ApplicationName = this.Title;
                if (Settings.Default.DB_Connection_Type == 0)
                {
                    My_CommObject.Protocol = "HTTP";
                }
                else
                {
                    My_CommObject.Protocol = "TCPIP";
                }
                My_CommObject.Server = Settings.Default.SQL_ServerName;
                My_CommObject.Database = Settings.Default.SQL_DatabaseName;
                if (Settings.Default.SQL_Authentication_Type == 0)
                {
                    My_CommObject.IntegratedSecurity = true;
                }
                else
                {
                    My_CommObject.IntegratedSecurity = false;
                }
                My_CommObject.Port = Settings.Default.SQL_Port;
                My_CommObject.TimeOut = Settings.Default.SQL_TimeOut;
                My_CommObject.UserID = Settings.Default.SQL_UserName;
                My_CommObject.Password = My_Functions.DecryptDES(Settings.Default.SQL_Password, My_Functions.encryptKey);
                My_CommObject.WebServiceURL = Settings.Default.WebServiceURL;
                My_CommObject.ProxyServer = Settings.Default.ProxyName;
                My_CommObject.ProxyPort = Settings.Default.ProxyPort;
                My_CommObject.ProxyVerify = Settings.Default.VerifyProxyUserNamePassword;
                My_CommObject.ProxyUser = Settings.Default.ProxyUserName;
                My_CommObject.ProxyPassword = My_Functions.DecryptDES(Settings.Default.ProxyPassword, My_Functions.encryptKey);
                My_CommObject.CertificateKEY = gsCertificateKEY;
                My_CommObject.Connect(ref sErr); //开始连接
                if (sErr != "")
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(sErr, this.Title, MessageBoxButton.OK);
                    radioButton.Focus();
                    return;
                }

                // used to decide what to do with incoming connections
                port = Settings.Default.Port;
                CertificateFile = Settings.Default.CertificateFile;
                CertificatePassword = Settings.Default.CertificatePassword;
                CertificatePassword = My_Functions.DecryptDES(CertificatePassword, My_Functions.encryptKey);
                Identification_Alias = Settings.Default.Identification_Alias;

                serviceFactory = new ServiceFactory(webRoot, _logger, Identification_Alias, server, My_CommObject);
                server = new WebServer(serviceFactory, _logger);
                serviceFactory._server = server;
                serviceFactory._My_CommObject = My_CommObject;
                if (Settings.Default.HTTPS)
                // if (port == 443)
                {
                    X509Certificate2 cert = GetCertificate(webRoot + Settings.Default.CertificateFile, My_Functions.DecryptDES(Settings.Default.CertificatePassword, My_Functions.encryptKey), ref sErr);
                    server.Listen(port, cert);
                }
                else
                {
                    server.Listen(port);
                }

                this.Cursor = Cursors.Arrow;
                radioButton.IsEnabled = false;
                cmdStop_Serivce.IsEnabled = true;
                cmdStop_Serivce.Focus();
                cmdViewLog.IsEnabled = true;
                // timer1.Enabled = true;
                cmdTest.IsEnabled = true;
                // pnlSC.BackColor = Color.Blue;
                picOrange.Visibility = Visibility.Hidden;
                picGreen.Visibility = Visibility.Visible;
                txtDevices.Text = "0";
                txtSubscribers.Text = "0";
                listView1.Items.Clear();
                msCurrentSelectedDeviceSN = "";
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void cmdViewLog_Click(object sender, EventArgs e)
        {

            try
            {
                String LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!System.IO.File.Exists(LogFileName))
                {
                    MessageBox.Show("No log file found!", this.Title, MessageBoxButton.OK);
                    return;
                }
                Process.Start(LogFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void cmdStop_Serivce_Click(object sender, EventArgs e)
        {
            try
            {
                String sErr = "";
                txtDevices.Text = "";
                // timer1.Enabled = false;
                server.Dispose();
                server = null;
                serviceFactory = null;
                _logger = null;

                My_CommObject.Close(ref sErr);
                My_CommObject = null;

                cmdStart_Service.IsEnabled = true;
                cmdStart_Service.Focus();
                cmdStop_Serivce.IsEnabled = false;
                cmdViewLog.IsEnabled = false;

                cmdTest.IsEnabled = false;
                picOrange.Visibility = Visibility.Visible;
                picGreen.Visibility = Visibility.Hidden;
                // pnlSC.BackColor = Color.PaleGreen;
                txtDevices.Text = "";
                txtSubscribers.Text = "";
                listView1.Items.Clear();
                msCurrentSelectedDeviceSN = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
                cmdStart_Service.IsEnabled = true;
                cmdStop_Serivce.IsEnabled = false;
                cmdViewLog.IsEnabled = false;
                // timer1.Enabled = false;
                cmdTest.IsEnabled = false;
                picOrange.Visibility = Visibility.Visible;
                picGreen.Visibility = Visibility.Hidden;
                listView1.Items.Clear();
               // pnlSC.BackColor = Color.PaleGreen;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (server.NeedtoRefresh == true)
            {
                server.NeedtoRefresh = false;
                TotalDevices = server.TotalDevices;
                txtDevices.Text = "0";
                txtSubscribers.Text = "0";
                String sTemp = "";
                try
                {
                    if (server != null)
                    {
                        sTemp = server.GetDevices_Info();
                        if (sTemp != "")
                        {
                            //txtDevices.Text = sTemp.Split('\r').Length.ToString();
                            txtDevices.Text = sTemp.Split('|')[0];
                            mnCounter += 1;
                            if (mnCounter > 0)//暂时不延时
                            {
                                mnCounter = 0;
                                RefreshGrid(sTemp);
                            }
                        }
                        //server.SendRawData("", "", 2000);//给打印机发送指令
                        sTemp = server.GetSubscribers_Info();
                        if (sTemp != "")
                        {
                            txtSubscribers.Text = sTemp.Split('|')[0];
                        }

                    }

                }
                catch (Exception ex)
                {
                    _logger.Error("", typeof(MainWindow), ex);
                }
            }


        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            try
            {
                Thread clientThread = new Thread(new ParameterizedThreadStart(TestClient));
                clientThread.IsBackground = false;

                // to enable ssl change the port to 443 in the settings file and use the wss schema below
                clientThread.Start("wss://localhost" + Identification_Alias);
                MessageBox.Show("Successfully send test message to the server, please check the log for more details", this.Title, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private static void TestClient(object state)
        {
            try
            {
                string url = (string)state;
                using (var client = new ChatWebSocketClient(true, _logger))
                {
                    Uri uri = new Uri(url);
                    client.TextFrame += Client_TextFrame;
                    client.ConnectionOpened += Client_ConnectionOpened;
                    // test the open handshake
                    client.OpenBlocking(uri);
                }

                _logger.Information("", typeof(MainWindow), "Client finished.");
            }
            catch (Exception ex)
            {
                _logger.Error("", typeof(MainWindow), ex.ToString());
                _logger.Information("", typeof(MainWindow), "Client terminated.");
            }
        }

        private static void Client_ConnectionOpened(object sender, EventArgs e)
        {
            _logger.Information("", typeof(MainWindow), "Client: Connection Opened");
            var client = (ChatWebSocketClient)sender;
            // test sending a message to the server
            if (msFMT_Test_Label != "")
            {
                client.Send(msFMT_Test_Label);
            }
            else
            {
                client.Send("Hi");
            }

        }

        private static void Client_TextFrame(object sender, TextFrameEventArgs e)
        {
            _logger.Information("", typeof(MainWindow), "Client: {0}", e.Text);
            var client = (ChatWebSocketClient)sender;

            // lets test the close handshake
            client.Dispose();
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            msCurrentSelectedDeviceSN = "";
            if (this.listView1.SelectedItems.Count > 0)
            {
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            msCurrentSelectedDeviceSN = "";
            // user clicked an item of listview control
            if (listView1.SelectedItems.Count == 1)
            {               
                DeviceDetails frm = new DeviceDetails(this);
                frm.Title = "Device:" + msCurrentSelectedDeviceSN;
                frm.ShowDialog();
            }
        } 

        public static string json_Create_message_for_PrintLabel(String sSN, String sData)
        {
            String ActionName = "Label";
            String ActionCommand = sSN;
            JObject postedJObject = new JObject();
            postedJObject.Add(ActionName, ActionCommand);
            String ActionCommand1 = sData;
            JObject postedJObject1 = new JObject();
            postedJObject.Add(ActionName, ActionCommand1);
            String paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
            return paramString;
        }

        public String SendRawDataToPrinter(String sData, String sDeviceSN)
        {
            String sTemp = server.SendRawData(sDeviceSN, sData, 1500);
            return sTemp;
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                FrmSettings frm = new FrmSettings();
                frm.Title = "Settings";
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }

        private void dialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeviceDetails deviceDetails = new DeviceDetails(this);
                deviceDetails.Title = "Device:" + msCurrentSelectedDeviceSN;
                deviceDetails.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK);
            }
        }
    }
}
