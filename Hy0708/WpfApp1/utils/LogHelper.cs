﻿using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.utils
{
    class LogHelper
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        public static void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            loginfo.Info("日志初始化");
        }

        public static void WriteInfoLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
                Console.WriteLine(info);
            }
        }

        public static void WriteErrLog(string info, Exception ex)
        {
            /*            if (logerror.IsErrorEnabled)
                        {
                            logerror.Error(info, ex);
                        }*/
            MyLogger.LogWrite(ex);
        }
    }
}
