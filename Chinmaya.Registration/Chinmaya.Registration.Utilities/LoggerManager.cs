using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Chinmaya.Registration.Utilities
{
    public class LoggerManager
    {
        private readonly ILog _log;
        public enum LogLevel
        {
            DEBUG, INFO, WARN, ERROR, FATAL }
        
        public LoggerManager(Type moduleType)
        {
            _log = LogManager.GetLogger(moduleType);
        }
        public void LogMessage(LogLevel lvl,string message)
        {
            switch(lvl)
            {
                case LogLevel.DEBUG:
                    _log.Debug(message);
                    break;
                case LogLevel.INFO:
                    _log.Info(message);
                    break;
                case LogLevel.WARN:
                    _log.Warn(message);
                    break;
            }
        }
        public void ErrorMessage(LogLevel lvl,string message, Exception ex = null )
        {
            if (ex != null)
            {
                if (lvl == LogLevel.ERROR)
                    _log.Error(message, ex);
                else if (lvl == LogLevel.FATAL)
                    _log.Fatal(message, ex);
            }
            else
            {
                if (lvl == LogLevel.ERROR)
                    _log.Error(message);
                else if (lvl == LogLevel.FATAL)
                    _log.Fatal(message);
            }
        }
    }
}
