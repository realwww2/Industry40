using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using I4.LocalConfig;
using I4.BaseCore;

namespace I4.LocalCapture
{
    class Program
    {
        //LocalCapture LocalCapture.config.xml
        static void Main(string[] args)
        {
            //Console.WriteLine("test"); Console.ReadLine();//for test
            Director director;
            try
            {
                if (args.GetLength(0) != 1)
                {
                    throw new LocalCaptureException("args of main function must be 1");
                }
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                basePath = basePath.Remove(basePath.Length - 1);
                Config config = new Config(basePath, args[0]);

                AppGlobal.Instance.LoadAppGlobal(basePath, config.GetKeyValue("LogError"), config.GetKeyValue("LogDebug"));

                director = new Director(config);
                director.Init();
                director.Execute();
                
            }
            catch (Exception ex)
            {
                AppGlobal.Instance.Logger.LogError(ex);
            }
            finally
            {
                //director.Close();
                System.Environment.Exit(0);
            }

        }
    }
}
