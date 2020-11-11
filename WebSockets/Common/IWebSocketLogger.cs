using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSockets.Common
{
    public interface IWebSocketLogger
    {
        void Information(String LogFileName, Type type, string format, params object[] args);
        void Warning(String LogFileName,Type type, string format, params object[] args);
        void Error(String LogFileName,Type type, string format, params object[] args);
        void Error(String LogFileName,Type type, Exception exception);
        void Events(String LogFileName, Type type, string format, params object[] args);
    }
}
