using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Diagnostics;
using WebSockets.Server;
using WebSockets.Server.Http;
using WebSockets.Common;
using WebSockets;
using COMM;

namespace WebSocketsCmd.Server
{
    internal class ServiceFactory : IServiceFactory
    {
        private readonly IWebSocketLogger _logger;
        public WebServer _server = null;
        public BusinessObject _My_CommObject = null;
        private readonly string _webRoot;
        private String _Identification_Alias = "";
        private string GetWebRoot()
        {
            if (!string.IsNullOrWhiteSpace(_webRoot) && Directory.Exists(_webRoot))
            {
                return _webRoot;
            }

            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", string.Empty);
        }


        public ServiceFactory(string webRoot, IWebSocketLogger logger, String Identification_Alias, WebServer server, BusinessObject My_CommObject)
        {
            _Identification_Alias = Identification_Alias;
            _logger = logger;
            _webRoot = string.IsNullOrWhiteSpace(webRoot) ? GetWebRoot() : webRoot;
            _server = server;
            _My_CommObject = My_CommObject;
            if (!Directory.Exists(_webRoot))
            {
                _logger.Warning("", this.GetType(), "Web root not found: {0}", _webRoot);
            }
            else
            {
                _logger.Information("", this.GetType(), "Web root: {0}", _webRoot);
            }
        }

        public IService CreateInstance(ConnectionDetails connectionDetails)
        {

            switch (connectionDetails.ConnectionType)
            {
                case ConnectionType.WebSocket:
                    // you can support different kinds of web socket connections using a different path
                    if (connectionDetails.Path == _Identification_Alias)
                    {
                        return new ChatWebSocketService(connectionDetails.Stream, connectionDetails.TcpClient, connectionDetails.Header, _logger, _server, _Identification_Alias, _My_CommObject);
                    }
                    break;
                case ConnectionType.Http:
                    // this path actually refers to the reletive location of some html file or image
                    return new HttpService(connectionDetails.Stream, connectionDetails.Path, _webRoot, _logger);
                case ConnectionType.HttpPost:
                    // Http Post
                    return new HttpPostService(connectionDetails.Stream, connectionDetails.TcpClient, connectionDetails.Header, _logger, _server, _Identification_Alias, _My_CommObject);
            }
            return new BadRequestService(connectionDetails.Stream, connectionDetails.Header, _logger);
        }

    }
}
