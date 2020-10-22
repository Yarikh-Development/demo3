using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSockets.Client;
using WebSockets.Common;

namespace WebSocketsCmd.Client
{
    class ChatWebSocketClient : WebSocketClient
    {
        public ChatWebSocketClient(bool noDelay, IWebSocketLogger logger) : base(noDelay, logger)
        {

        }

        public new void Send(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            base.Send(WebSocketOpCode.TextFrame, buffer);
        }
    }
}