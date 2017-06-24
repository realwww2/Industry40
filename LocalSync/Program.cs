using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;
using I4.LocalConfig;
using I4.Log;

namespace I4.LocalSync
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("test"); Console.ReadLine();//for test
            Director director;
            try
            {
                if (args.GetLength(0) != 1)
                {
                    throw new LocalSyncException("args of main function must be 1");
                }
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                basePath = basePath.Remove(basePath.Length - 1);
                Config config = new Config(basePath, args[0]);

                AppGlobal.Instance.LoadAppGlobal(basePath, config.GetKeyValue("LogError"), config.GetKeyValue("LogDebug"));
                AppGlobal.Instance.Logger.LogDebug("LocalCapture is starting...\n");
                director = new Director(config);
                director.Init();
                director.Execute();
                while (true)
                {
                    string cmd = Console.ReadLine();
                    if (cmd.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        return;
                }
            }
            catch (Exception ex)
            {
                AppGlobal.Instance.Logger.LogError(ex);
            }
            finally
            {
                //director.Close();
                AppGlobal.Instance.Logger.LogDebug("LocalCapture is ending...\n");
                System.Environment.Exit(0);
            }
        }
    }
}
