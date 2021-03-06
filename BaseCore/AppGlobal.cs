﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.Log;

namespace I4.BaseCore
{
    public class AppGlobal
    {

        public static AppGlobal Instance = new AppGlobal();
        private AppGlobal()
        {
        }

        private string _basePath;
        public void LoadAppGlobal(string basePath,string logErrorStr,string logDebugStr)
        {
            _basePath = basePath;
            _logger = new Logger(string.Format("{0}\\Log", _basePath),logErrorStr,logDebugStr);
        }
        public string Path2Full(string fileNameOnly)
        {
            return string.Format("{0}\\{1}", _basePath, fileNameOnly);
        }
        public string Path2FullConfig(string fileNameOnly)
        {
            return string.Format("{0}\\ConfigFiles\\{1}", _basePath, fileNameOnly);
        }
        private Logger _logger;
        public Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    basePath = basePath.Remove(basePath.Length - 1);
                    _logger = new Logger(string.Format("{0}\\log", basePath),"AppStartError.log","AppStartDebug.log");
                }
                return _logger;
            }
        }

    }
}
