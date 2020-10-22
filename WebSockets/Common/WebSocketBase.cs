using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using WebSockets.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Threading;

namespace WebSockets.Common
{
    public abstract class WebSocketBase
    {
        private readonly IWebSocketLogger _logger;
        private readonly object _sendLocker;
        private Stream _stream;
        private WebSocketFrameWriter _writer;
        private WebSocketOpCode _multiFrameOpcode;
        private Socket _socket;
        protected bool _isOpen;

        public String _ClientName="";
        public String _ClientID="";
        public String _ClientVer="";
        public String _ClientLocation = "";
        public String _ClientUnique_id = "";
        public String _ClientChannel_name = "";
        public String _ClientChannel_id = "";
        public String _Client_Alert_Unique_id = "";
        public String _Client_Alert_Time_stamp = "";
        public String _Client_Alert_Type_id = "";
        public String _Client_Alert_Condition_id = "";
        public String _Client_Alert_Condition_state = "";
        public String _Client_Alert_Type = "";
        public String _Client_Alert_Condition = "";
        public String _Client_IP_Local = "";
        public String _Client_IP_Gateway = "";
        public String _Client_Type = "";
        public String _Client_Message_Buffer = "";

        public event EventHandler ConnectionOpened;
        public event EventHandler<ConnectionCloseEventArgs> ConnectionClose;
        public event EventHandler<PingEventArgs> Ping;
        public event EventHandler<PingEventArgs> Pong;
        public event EventHandler<TextFrameEventArgs> TextFrame;
        public event EventHandler<TextMultiFrameEventArgs> TextMultiFrame;
        public event EventHandler<BinaryFrameEventArgs> BinaryFrame;
        public event EventHandler<BinaryMultiFrameEventArgs> BinaryMultiFrame;


        public  bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string json_DeserializeObject_Job(string jsonText)
        {
            String sTemp = "";
            //dynamic d = JObject.Parse("{number:1000, str:'string', array: [1,2,3,4,5,6]}");
            Dictionary<string, string> responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            foreach (string key in responseJson.Keys)
            {
                sTemp = key.ToString() ;
                JToken discoveryContent = null;
                JObject JSearch = JObject.Parse(jsonText);
                JSearch.TryGetValue(sTemp, out discoveryContent);
                sTemp += "|" + discoveryContent;
            }
            return sTemp;

        }

        public static string json_DeserializeObject_Package_Type(string jsonText)
        {
            String sTemp = "";
            Dictionary<string, string> responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            foreach (string key in responseJson.Keys)
            {
                sTemp = key.ToString();
             }
            return sTemp;
            
        }

        public static string json_DeserializeObject_Alert(string jsonText)
        {
            String receivedDiscoveryContent = "";
            JToken discoveryContent = null;
            JObject JSearch = JObject.Parse(jsonText);
            JSearch.TryGetValue("alert", out discoveryContent);
            Dictionary<string, string> responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(discoveryContent.ToString());
            string sTemp = string.Empty;

            responseJson.TryGetValue("unique_id", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("time_stamp", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("type_id", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("condition_id", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("condition_state", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("type", out sTemp);
            receivedDiscoveryContent += sTemp + "|";

            responseJson.TryGetValue("condition", out sTemp);
            receivedDiscoveryContent += sTemp + "|";
            


            return receivedDiscoveryContent;
        }

        public static string json_DeserializeObject_Package_Type_NEW(string jsonText)
        {
            String sTemp = "";
            try
            {
                JToken discoveryContent = null;
                JObject JSearch = JObject.Parse(jsonText);
                if (JSearch.TryGetValue("discovery_b64", out discoveryContent))
                {
                    sTemp = "discovery_b64";
                    return sTemp;
                }
                if (JSearch.TryGetValue("channel_id", out discoveryContent))
                {
                    sTemp = "channel_id";
                    return sTemp;
                }
                if (JSearch.TryGetValue("alert", out discoveryContent))
                {
                    sTemp = "alert";
                    return sTemp;
                }
            }
            catch 
            {
                sTemp = jsonText;
            }
            return sTemp;
        }

        public static string json_DeserializeObject_discovery_b64(string jsonText)
        {
            Dictionary<string, string> responseJson =JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            string discovery64Content = string.Empty;
            string receivedDiscoveryContent = string.Empty;
            if (responseJson.TryGetValue("discovery_b64", out discovery64Content))
            {
                string encoded64 = discovery64Content.Split(':')[0];
                byte[] data = Convert.FromBase64String(encoded64);
                receivedDiscoveryContent = Encoding.UTF8.GetString(data);
                receivedDiscoveryContent = receivedDiscoveryContent.Replace("\0", "|");
                receivedDiscoveryContent = receivedDiscoveryContent.Replace("�", "");
                receivedDiscoveryContent = new string(receivedDiscoveryContent.Where(c => !char.IsControl(c)).ToArray());
                do
                 {
                    receivedDiscoveryContent = receivedDiscoveryContent.Replace("||", "|");
                  } while (receivedDiscoveryContent.IndexOf("||") > 0);
            }
            return receivedDiscoveryContent;
        }

        public static string json_DeserializeObject_RawChannel(string jsonText)
        {
            Dictionary<string, string> responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            string sTemp = string.Empty;
            string sRetrun = string.Empty;
            responseJson.TryGetValue("unique_id", out sTemp);
            sRetrun +=  sTemp + "|";

            responseJson.TryGetValue("channel_name", out sTemp);
            sRetrun +=  sTemp + "|";

            responseJson.TryGetValue("channel_id", out sTemp);
            sRetrun +=  sTemp + "|";

            return sRetrun;
        }

        public static string json_Create_message_for_Open_Raw_Channel()
        {
            String ActionName = "open";
            String ActionCommand = "v1.raw.zebra.com";
            JObject postedJObject = new JObject();
            postedJObject.Add(ActionName, ActionCommand);
            String paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
            return paramString;
         }

        public static string json_Create_message_for_Open_Configuration_Channel()
        {
            String ActionName = "open";
            String ActionCommand = "v1.config.zebra.com";
            JObject postedJObject = new JObject();
            postedJObject.Add(ActionName, ActionCommand);
            String paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
            return paramString;
        }

        public WebSocketBase(IWebSocketLogger logger)
        {
            _logger = logger;
            _sendLocker = new object();
            _isOpen = false;
        }

        protected void OpenBlocking(Stream stream, Socket socket, bool isClient)
        {
            _socket = socket;
            _stream = stream;
            _writer = new WebSocketFrameWriter(stream, isClient);
            PerformHandshake(stream);
            _isOpen = true;
            MainReadLoop();
        }

        protected virtual void Send(WebSocketOpCode opCode, byte[] toSend, bool isLastFrame)
        {
            if (_isOpen)
            {
                lock (_sendLocker)
                {
                    if (_isOpen)
                    {
                        _writer.Write(opCode, toSend, isLastFrame);
                    }
                }
            }
        }

        protected virtual void Send(WebSocketOpCode opCode, byte[] toSend)
        {
            Send(opCode, toSend, true);
        }

        protected virtual void Send(byte[] toSend)
        {
            Send(WebSocketOpCode.BinaryFrame, toSend, true);
        }

       public  virtual void Send(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Send(WebSocketOpCode.TextFrame, bytes, true);
        }

        public virtual void Send_BinaryFrame(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Send(WebSocketOpCode.BinaryFrame, bytes, true);
        }

        protected virtual void OnConnectionOpened()
        {
            if (ConnectionOpened != null)
            {
                ConnectionOpened(this, new EventArgs());
            }
        }

        protected virtual void OnPing(byte[] payload)
        {
            Send(WebSocketOpCode.Pong, payload);

            if (Ping != null)
            {
                Ping(this, new PingEventArgs(payload));
            }
        }

        protected virtual void OnPong(byte[] payload)
        {
            if (Pong != null)
            {
                Pong(this, new PingEventArgs(payload));
            }
        }

        protected virtual void OnTextFrame(string text)
        {
            if (TextFrame != null)
            {
                TextFrame(this, new TextFrameEventArgs(text));
            }
        }

        protected virtual void OnTextMultiFrame(string text, bool isLastFrame)
        {
            if (TextMultiFrame != null)
            {
                TextMultiFrame(this, new TextMultiFrameEventArgs(text, isLastFrame));
            }
        }

        protected virtual void OnBinaryFrame(byte[] payload,ref int nStatus)
        {
            nStatus = 0;
            if (BinaryFrame != null)
            {
                BinaryFrame(this, new BinaryFrameEventArgs(payload));
                nStatus = 1;
            }
        }

        protected virtual void OnBinaryMultiFrame(byte[] payload, bool isLastFrame)
        {
            if (BinaryMultiFrame != null)
            {
                BinaryMultiFrame(this, new BinaryMultiFrameEventArgs(payload, isLastFrame));
            }
        }

        protected virtual void OnConnectionClose(byte[] payload)
        {
            ConnectionCloseEventArgs args = GetConnectionCloseEventArgsFromPayload(payload);

            if (args.Reason == null)
            {
                _logger.Information("",this.GetType(), "Received web socket close message: {0}", args.Code);
            }
            else
            {
                _logger.Information("",this.GetType(), "Received web socket close message: Code '{0}' Reason '{1}'", args.Code, args.Reason);
            }

            if (ConnectionClose != null)
            {
                ConnectionClose(this, args);
            }
        }

        protected abstract void PerformHandshake(Stream stream);

        /// <summary>
        /// Combines the key supplied by the client with a guid and returns the sha1 hash of the combination
        /// </summary>
        protected string ComputeSocketAcceptString(string secWebSocketKey)
        {
            // this is a guid as per the web socket spec
            const string webSocketGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

            string concatenated = secWebSocketKey + webSocketGuid;
            byte[] concatenatedAsBytes = Encoding.UTF8.GetBytes(concatenated);
            byte[] sha1Hash = SHA1.Create().ComputeHash(concatenatedAsBytes);
            string secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            return secWebSocketAccept;
            
        }

        protected ConnectionCloseEventArgs GetConnectionCloseEventArgsFromPayload(byte[] payload)
        {
            if (payload.Length >= 2)
            {
                using (MemoryStream stream = new MemoryStream(payload))
                {
                    ushort code = BinaryReaderWriter.ReadUShortExactly(stream, false);

                    try
                    {
                        WebSocketCloseCode closeCode = (WebSocketCloseCode)code;

                        if (payload.Length > 2)
                        {
                            string reason = Encoding.UTF8.GetString(payload, 2, payload.Length - 2);
                            return new ConnectionCloseEventArgs(closeCode, reason);
                        }
                        else
                        {
                            return new ConnectionCloseEventArgs(closeCode, null);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        _logger.Warning("",this.GetType(), "Close code {0} not recognised", code);
                        return new ConnectionCloseEventArgs(WebSocketCloseCode.Normal, null);
                    }
                }
            }

            return new ConnectionCloseEventArgs(WebSocketCloseCode.Normal, null);
        }

        private void MainReadLoop()
        {
            int nTemp = 0;
            Stream stream = _stream;
            OnConnectionOpened();
            WebSocketFrameReader reader = new WebSocketFrameReader();
            List<WebSocketFrame> fragmentedFrames = new List<WebSocketFrame>();

            while (true)
            {
                WebSocketFrame frame;

                try
                {
                    frame = reader.Read(stream, _socket);
                    if (frame == null)
                    {
                        return;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                
                // if we have received unexpected data
                if (!frame.IsValid)
                {
                    return;
                }

           
               //_logger.Events("",this.GetType(), "test");

                if (frame.OpCode == WebSocketOpCode.ContinuationFrame)
                {
                    switch (_multiFrameOpcode)
                    {
                        case WebSocketOpCode.TextFrame:
                            String data = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);
                            OnTextMultiFrame(data, frame.IsFinBitSet);
                            break;
                        case WebSocketOpCode.BinaryFrame:
                            OnBinaryMultiFrame(frame.DecodedPayload, frame.IsFinBitSet);
                            break;
                    }
                }
                else
                {
                    switch (frame.OpCode)
                    {
                        case WebSocketOpCode.ConnectionClose:
                            OnConnectionClose(frame.DecodedPayload);
                            return;
                        case WebSocketOpCode.Ping:
                            OnPing(frame.DecodedPayload);
                            break;
                        case WebSocketOpCode.Pong:
                            OnPong(frame.DecodedPayload);
                            break;
                        case WebSocketOpCode.TextFrame:
                            String data = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);
                            if (frame.IsFinBitSet)
                            {
                                OnTextFrame(data);
                            }
                            else
                            {
                                _multiFrameOpcode = frame.OpCode;
                                OnTextMultiFrame(data, frame.IsFinBitSet);
                            }
                            break;
                        case WebSocketOpCode.BinaryFrame:
                            nTemp = 0;
                            if (frame.IsFinBitSet)
                            {
                                OnBinaryFrame(frame.DecodedPayload, ref nTemp);
                                if (nTemp ==0)//有可能是打印机的数据包
                                {
                                    String discovery_data = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);
                                    _logger.Information("", this.GetType(), "Packet Received:{0}" , discovery_data);
                                    // String Package_Type = json_DeserializeObject_Package_Type(discovery_data);
                                    String Package_Type = json_DeserializeObject_Package_Type_NEW(discovery_data);
                                    switch (Package_Type.ToLower())
                                    {
                                        //When the main channel (a channel is essentially another name for a connection) is connected, the printer sends the server the Zebra Discovery Packet via a JSON message
                                        case "discovery_b64"://Zebra Printer
                                            discovery_data = json_DeserializeObject_discovery_b64(discovery_data);
                                            _ClientID = discovery_data.Split('|')[6];
                                            _ClientName = discovery_data.Split('|')[2];
                                            _ClientVer = discovery_data.Split('|')[4];
                                            _ClientLocation= discovery_data.Split('|')[5];
                                            _Client_IP_Gateway = _socket.RemoteEndPoint.ToString().Split(':')[0];
                                            _Client_Type = "Printer";
                                            //request the Raw Channel
                                            discovery_data = json_Create_message_for_Open_Raw_Channel();
                                            Send_BinaryFrame(discovery_data);
                                            _logger.Information("", this.GetType(), "Send: Request the Raw Channel.");

                                            Thread.Sleep(10);

                                            //request the Configuration Channel
                                            discovery_data = json_Create_message_for_Open_Configuration_Channel();
                                            Send_BinaryFrame(discovery_data);
                                            _logger.Information("", this.GetType(), "Send: Request the Configuration Channel.");

                                           

                                            break;
                                        case "channel_id":
                                            discovery_data = json_DeserializeObject_RawChannel(discovery_data);
                                            _ClientUnique_id = discovery_data.Split('|')[0];
                                            _ClientChannel_name = discovery_data.Split('|')[1];
                                            _ClientChannel_id = discovery_data.Split('|')[2];

                                            //Thread.Sleep(10);
                                            //request the Device IP  注:需要在RAW或Config通道发送ZPL或SDG
                                            //Send_BinaryFrame("! U1 getvar " + @"""interface.network.active.ip_addr""" + Environment.NewLine);
                                            //_logger.Information("", this.GetType(), "Send: Request Device IP Address.");

                                            break;
                                        case "alert":
                                            discovery_data = json_DeserializeObject_Alert(discovery_data);
                                            _Client_Alert_Unique_id = discovery_data.Split('|')[0];
                                            _Client_Alert_Time_stamp = discovery_data.Split('|')[1];
                                            _Client_Alert_Type_id = discovery_data.Split('|')[2];
                                            _Client_Alert_Condition_id = discovery_data.Split('|')[3];
                                            _Client_Alert_Condition_state = discovery_data.Split('|')[4];
                                            _Client_Alert_Type = discovery_data.Split('|')[5];
                                            _Client_Alert_Condition = discovery_data.Split('|')[6];
                                            break;
                                        default:
                                            //discovery_data = JsonConvert.DeserializeObject(discovery_data).ToString();
                                            _Client_Message_Buffer += discovery_data ;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                _multiFrameOpcode = frame.OpCode;
                                OnBinaryMultiFrame(frame.DecodedPayload, frame.IsFinBitSet);
                            }
                            break;
                    }
                }
            }
        }
    }
}
