using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.Log;
using I4.LocalConfig;

namespace I4.LocalCapture
{
    class AppGlobal
    {

        public static AppGlobal Instance = new AppGlobal();
        private AppGlobal()
        {
        }

        private string _basePath;
        public void LoadAppGlobal(string basePath)
        {
            _basePath = basePath;
            _logger = new Logger(string.Format("{0}\\Log", _basePath));
        }
        public string Path2Full(string fileNameOnly)
        {
            return string.Format("{0}\\{1}", _basePath, fileNameOnly);
        }
        private Logger _logger;
        public Logger Logger
        {
            get { return _logger; }
        }

    }
}
