using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Sockets;
using WebSockets.Server.WebSocket;
using WebSockets.Common;
using System.IO;
using WebSockets.Server;
using WebSockets.Events;
using WebSockets;
using System.Data;
using COMM;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketsCmd.Server
{
    internal class ChatWebSocketService : WebSocketService
    {
        private readonly IWebSocketLogger _logger;
        private WebServer _server = null;
        private BusinessObject _My_CommObject = null;
        private readonly String _Identification_Alias = "";
        public ChatWebSocketService(Stream stream, TcpClient tcpClient, string header, IWebSocketLogger logger, WebServer server, String Identification_Alias, BusinessObject My_CommObject) : base(stream, tcpClient, header, true, logger)
        {
            _logger = logger;
            _server = server;
            _Identification_Alias = Identification_Alias;
            _My_CommObject = My_CommObject;
        }

        public static bool Handle_Devices_Alerts(string Site_Name, string Device_SN, string Alert_Type, string Alert_Message, string Alert_Condition_ID, string Alert_Condition_State, BusinessObject My_CommObject, ref String sErr)
        {
            sErr = "";
            try
            {
                DataSet ds = null;
                ds = My_CommObject.ExecuteStoredProcedure("sp_Handle_Devices_Alerts", "@Site_Name|nvarchar|" + Site_Name + ";@Device_SN|nvarchar|" + Device_SN + ";@Alert_Type|nvarchar|" + Alert_Type + ";@Alert_Message|nvarchar|" + Alert_Message + ";@Alert_Condition_ID|nvarchar|" + Alert_Condition_ID + ";@Alert_Condition_State|nvarchar|" + Alert_Condition_State + ";@RetCode|nvarchar||18|OUT" + ";@RetMsg|nvarchar||255|OUT", ref sErr);
                if (sErr == "")
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            sErr = ds.Tables[0].Rows[1][0].ToString();
                            if (sErr != "")
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                return false;
            }
        }

        public static bool Handle_Devices_Connection_Events(string Site_Name, string Event_Type, string Device_Type, string Device_Name, string Device_SN, string Device_FW, string Device_Location, string Last_Login_IP, string Last_Alert_Message, string Device_Rev_1, string Device_Rev_2, string Device_Rev_3, BusinessObject My_CommObject, ref String sErr)
        {
            sErr = "";
            try
            {
                DataSet ds = null;
                ds = My_CommObject.ExecuteStoredProcedure("sp_Handle_Devices_Connection_Events", "@Site_Name|nvarchar|" + Site_Name + ";@Event_Type|nvarchar|" + Event_Type + ";@Device_Type|nvarchar|" + Device_Type + ";@Device_Name|nvarchar|" + Device_Name + ";@Device_SN|nvarchar|" + Device_SN + ";@Device_FW|nvarchar|" + Device_FW + ";@Device_Location|nvarchar|" + Device_Location + ";@Last_Login_IP|nvarchar|" + Last_Login_IP + ";@Last_Alert_Message|nvarchar|" + Last_Alert_Message + ";@Device_Rev_1|nvarchar|" + Device_Rev_1 + ";@Device_Rev_2|nvarchar|" + Device_Rev_2 + ";@Device_Rev_3|nvarchar|" + Device_Rev_3 + ";@RetCode|nvarchar||18|OUT" + ";@RetMsg|nvarchar||255|OUT", ref sErr);
                if (sErr == "")
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            sErr = ds.Tables[0].Rows[1][0].ToString();
                            if (sErr != "")
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                return false;
            }
        }

        protected override void OnTextFrame(string text)
        {
            try
            {
                string response = "Server Send: " + text;
                base.Send(response);

                // limit the log message size
                int length = 1024 * 16; // 16KB buffer more than enough for http header
                string logMessage = response.Length > length ? response.Substring(0, length) + "..." : response;
                _logger.Information("", this.GetType(), logMessage);
            }
            catch (Exception ex)
            {
                _logger.Error("", this.GetType(), ex.Message);
            }
        }

        protected override void OnBinaryFrame(byte[] payload, ref int nStatus)
        {
            try
            {
                String discovery_data = Encoding.UTF8.GetString(payload, 0, payload.Length);
                String Package_Type = json_DeserializeObject_Package_Type_NEW(discovery_data);
                String sErr = "";
                switch (Package_Type.ToLower())
                {
                    case "alert":
                        _server.NeedtoRefresh = true;//让主界面重新刷新
                        discovery_data = json_DeserializeObject_Alert(discovery_data);
                        //_Client_Alert_Unique_id = discovery_data.Split('|')[0];
                        //_Client_Alert_Time_stamp = discovery_data.Split('|')[1];
                        //_Client_Alert_Type_id = discovery_data.Split('|')[2];
                        //_Client_Alert_Condition_id = discovery_data.Split('|')[3];
                        //_Client_Alert_Condition_state = discovery_data.Split('|')[4];
                        //_Client_Alert_Type = discovery_data.Split('|')[5];
                        //_Client_Alert_Condition = discovery_data.Split('|')[6];
                        Handle_Devices_Alerts(_Identification_Alias, discovery_data.Split('|')[0], discovery_data.Split('|')[2], discovery_data.Split('|')[6], discovery_data.Split('|')[3], discovery_data.Split('|')[4], _My_CommObject, ref sErr);
                        if (sErr != "")
                        {
                            _logger.Error("", this.GetType(), sErr);
                        }
                        break;
                    default:
                        break;
                }
                //string response = "Server Send: " + text;
                //base.Send(response);  
            }
            catch (Exception ex)
            {
                _logger.Error("", this.GetType(), ex.Message);
            }
        }
        protected override void OnConnectionOpened()
        {
            try
            {
                String sTemp = "";
                String sErr = "";
                _server.TotalDevices = _server.TotalDevices + 1;
                _server.NeedtoRefresh = true;
                sTemp = _server.GetDevices_Info();
                if (sTemp.Split('|').Length > 2)
                {
                    // sTemp.Split('|')[2] PrinterName
                    // sTemp.Split('|')[1] SerialNumber
                    // sTemp.Split('|')[3] FWVersion
                    // sTemp.Split('|')[4] Location
                    // sTemp.Split('|')[5] IP
                    // sTemp.Split('|')[7] Alert
                    Handle_Devices_Connection_Events(_Identification_Alias, "ConnectionOpened", sTemp.Split('|')[8], sTemp.Split('|')[2], sTemp.Split('|')[1], sTemp.Split('|')[3], sTemp.Split('|')[4], sTemp.Split('|')[5], sTemp.Split('|')[7], "", "", "", _My_CommObject, ref sErr);
                    if (sErr != "")
                    {
                        _logger.Error("", this.GetType(), sErr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("", this.GetType(), ex.Message);
            }
        }

        protected override void OnConnectionClose(byte[] payload)
        {
            try
            {
                String sTemp = "";
                String sErr = "";
                _server.TotalDevices = _server.TotalDevices - 1;
                _server.NeedtoRefresh = true;
                sTemp = _server.GetDevices_Info();
                if (sTemp.Split('|').Length > 2)
                {
                    // sTemp.Split('|')[2] PrinterName
                    // sTemp.Split('|')[1] SerialNumber
                    // sTemp.Split('|')[3] FWVersion
                    // sTemp.Split('|')[4] Location
                    // sTemp.Split('|')[5] IP
                    // sTemp.Split('|')[7] Alert
                    Handle_Devices_Connection_Events(_Identification_Alias, "ConnectionClosed", sTemp.Split('|')[8], sTemp.Split('|')[2], sTemp.Split('|')[1], sTemp.Split('|')[3], sTemp.Split('|')[4], sTemp.Split('|')[5], sTemp.Split('|')[7], "", "", "", _My_CommObject, ref sErr);
                    if (sErr != "")
                    {
                        _logger.Error("", this.GetType(), sErr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("", this.GetType(), ex.Message);
            }
        }


    }
}
