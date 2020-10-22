using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using WebSockets.Server.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketsCmd.Server
{
    internal class HttpPostService : WebSocketService
    {
        private readonly IWebSocketLogger _logger;
        private WebServer _server = null;
        private BusinessObject _My_CommObject = null;
        private readonly String _Identification_Alias = "";
        private string _header;
        private readonly Stream _stream;



        public void RespondSuccess(string contentType)
        {
            string response = "HTTP/1.1 200 OK" + Environment.NewLine +
                              "Content-Type: " + contentType + Environment.NewLine +
                              "Content-Length: " + "0" + Environment.NewLine +
                              "Connection: close";
           
            HttpHelper.WriteHttpHeader(response, _stream);
        }

        public static DataTable JasonToDatable(string json)
        {
            var jsonLinq = JObject.Parse(json);
            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }
                trgArray.Add(cleanRow);
            }
            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }

        public HttpPostService(Stream stream, TcpClient tcpClient, string header, IWebSocketLogger logger, WebServer server, String Identification_Alias, BusinessObject My_CommObject) : base(stream, tcpClient, header, true, logger)
        {
            _logger = logger;
            _server = server;
            _stream = stream;
            _Identification_Alias = Identification_Alias;
            _header = header;
            //Response to Client
            RespondSuccess("text/html");

            _My_CommObject = My_CommObject;
            string sValue = "";
            string sDeviceType = "";
            string[] tokens = _header.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
         
            if (tokens.Length >3)//标示有标识内容类型
            {
                foreach (string sTemp in tokens)
                {
                    if ((sTemp.ToUpper().Contains("application/json".ToUpper())) && (sTemp.ToUpper().Contains("Content-Type:".ToUpper())))//Jason format
                    {
                        sValue = tokens[tokens.Length - 1];
                        if (base.IsValidJson(sValue))
                        {
                            JToken Content = null;
                            JObject JSearch = JObject.Parse(sValue);
                            if (JSearch.TryGetValue("reader_name", out Content))//Zebra FX Reader
                            {
                                sDeviceType = "ZebraFXReader";
                            }
                        }
                    }

                    if ((sTemp.ToUpper().Contains("multipart/form-data;".ToUpper())) && (sTemp.ToUpper().Contains("Content-Type:".ToUpper())))//Jason format
                    {
                        sDeviceType = "ZebraPrinter_HttpPost";
                        sValue = tokens[tokens.Length - 1];
                    }

                }
                _logger.Information("", this.GetType(), "Got HttpPost request:" + Environment.NewLine + _header);
            }
            else
            {
                _logger.Information("", this.GetType(), "Invalid http request");
            }
        }
        public static bool Handle_Devices_Alerts(string Site_Name,string Device_SN, string Alert_Type, string Alert_Message, string Alert_Condition_ID, string Alert_Condition_State, BusinessObject My_CommObject, ref String sErr)
        {
            sErr = "";
            try
            {
                DataSet ds = null;
                ds = My_CommObject.ExecuteStoredProcedure("sp_Handle_Devices_Alerts", "@Site_Name|nvarchar|" + Site_Name +  ";@Device_SN|nvarchar|" + Device_SN + ";@Alert_Type|nvarchar|" + Alert_Type + ";@Alert_Message|nvarchar|" + Alert_Message + ";@Alert_Condition_ID|nvarchar|" + Alert_Condition_ID + ";@Alert_Condition_State|nvarchar|" + Alert_Condition_State + ";@RetCode|nvarchar||18|OUT" + ";@RetMsg|nvarchar||255|OUT", ref sErr);
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

       
    }
}
