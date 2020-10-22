using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSockets.Common;
using System.Diagnostics;

namespace WebSocketsCmd
{
    internal class WebSocketLogger : IWebSocketLogger
    {
        public void Events(String LogFileName, Type type, string format, params object[] args)
        {
            if (LogFileName == "")
            {
                LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            // limit the log message size
            string logMessage = format.Length > 1024 * 16 ? format.Substring(0, 1024 * 16) + "..." : format;
            //Trace.TraceInformation(logMessage, args);
            logMessage = string.Format(logMessage, args);
            System.IO.File.AppendAllText(LogFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + logMessage + Environment.NewLine);
        }

        public void Information(String LogFileName,Type type, string format, params object[] args)
        {
            if (LogFileName =="")
            {
                LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            string logMessage = format.Length > 1024 * 16 ? format.Substring(0, 1024 * 16) + "..." : format;
            // Trace.TraceInformation(logMessage, args);
            logMessage = string.Format(logMessage, args);
            System.IO.File.AppendAllText(LogFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + logMessage + Environment.NewLine);
        }

        public void Warning(String LogFileName, Type type, string format, params object[] args)
        {
            if (LogFileName == "")
            {
                LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            string logMessage = format.Length > 1024 * 16 ? format.Substring(0, 1024 * 16) + "..." : format;
            //Trace.TraceWarning(logMessage, args);
            logMessage = string.Format(logMessage, args);
            System.IO.File.AppendAllText(LogFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + logMessage + Environment.NewLine);
        }

        public void Error(String LogFileName, Type type, string format, params object[] args)
        {
            if (LogFileName == "")
            {
                LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            string logMessage = format.Length > 1024 * 16 ? format.Substring(0, 1024 * 16) + "..." : format;
            logMessage = string.Format(logMessage, args);
           // Trace.TraceError(logMessage, args);
            System.IO.File.AppendAllText(LogFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + logMessage + Environment.NewLine);
        }

        public void Error(String LogFileName, Type type, Exception exception)
        {
            if (LogFileName == "")
            {
                LogFileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            Error(LogFileName,type, "{0}", exception);
            System.IO.File.AppendAllText(LogFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + exception.Message  + Environment.NewLine);
        }
    }
}
