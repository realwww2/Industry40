using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;
using I4.LocalConfig;
using I4.Log;

namespace I4.LocalSchedule
{
    class Program
    {
        //LocalSchedule LocalSchedule.config.xml
        static void Main(string[] args)
        {
            try
            {
                if (args.GetLength(0) != 1)
                {
                    throw new LocalScheduleException("args of main function must be 1");
                }
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                Config config = new Config(basePath, args[0]);

                AppGlobal.Instance.LoadAppGlobal(basePath, config.GetKeyValue("LogError"), config.GetKeyValue("LogDebug"));

                Director director = new Director(config);
                director.Execute();

            }
            catch (Exception ex)
            {
                AppGlobal.Instance.Logger.LogError(ex);
            }
        }
    }
}
