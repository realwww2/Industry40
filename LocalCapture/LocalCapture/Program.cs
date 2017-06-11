using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using I4.LocalConfig;

namespace I4.LocalCapture
{
    class Program
    {
        //LocalCapture LocalCapture.config.xml
        static void Main(string[] args)
        {
            try
            {
                if (args.GetLength(0) != 1)
                {
                    throw new LocalCaptureException("args of main function must be 1");
                }
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                AppGlobal.Instance.LoadAppGlobal(basePath);
                Director director = new Director(new Config(basePath, args[0]));
                director.Execute();
            }
            catch (Exception ex)
            {
                AppGlobal.Instance.Logger.LogError(ex);
            }
            
        }
    }
}
