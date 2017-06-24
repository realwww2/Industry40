using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;
using I4.LocalConfig;
using I4.Log;
using System.Threading;
using System.Diagnostics;

namespace I4.LocalSchedule
{
    public class Director
    {
        private Config _config;
        public Director (Config config)
        {
            //create localcapture and localsync process, then execute and monitor
            _config = config;

        }

        //We must keep localcapture.exe and localsync.exe running forever, 
        //if localcapture.exe exit, restart it
        //if .localsyncexe exit, restart it
        public void Execute()
        {
            SendMsg("LocalSchedule start running...");
            Thread monitor = new Thread(this.ThreadMonitor);
            monitor.Start();
            Thread.Sleep(3000);
            Console.WriteLine("Press exit to kill ");
            while (!"exit".Equals(Console.ReadLine(),StringComparison.OrdinalIgnoreCase)) 
                System.Threading.Thread.Sleep(100);
            
        }
        

        private void ThreadMonitor(object obj)
        {
            GC.GetTotalMemory(true); // how much GC total use 
            string schedule = _config.GetKeyValue("Schedule");
            //here all process of ms are null
            MonitorClass[] ms = MonitorClass.Parse(schedule, _config);
            foreach (MonitorClass m in ms)
            {
                KillProcess(m.ProcessName);
                SendMsg(string.Format("{0} process will be started...", m.ToString()));
            }        
            do
            {
                ResetAllProcess(ms);
                System.Threading.Thread.Sleep(1000);
            } while (true);
        }

        private class MonitorClass
        {
            public string ProcessName { get; set; }
            public string Args { get; set; }
            public Process Process { get; set; }
            public MonitorClass(string processName, string args)
            {
                ProcessName = processName;
                Args = args;
            }
            public override string ToString()
            {
                return string.Format("{0} {1}", ProcessName, Args);
            }
            public static MonitorClass[] Parse(string schedule, Config config)
            {
                string[] arr = schedule.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                MonitorClass[] ms = new MonitorClass[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    string[] command = arr[i].Split(new char[] { ',' });
                    ms[i] = new MonitorClass(config.GetKeyValue(command[0]), config.GetKeyValue(command[1]));
                }
                return ms;
            }
        }

        private void ResetAllProcess(MonitorClass[] ms)
        {
            for (int i = 0; i < ms.GetLength(0); i++)
            {
                MonitorClass m = ms[i];
                if (IsResetProcess(m.Process))
                {
                    Process newProcess = CreateOneProcess(AppGlobal.Instance.Path2Full(m.ProcessName), m.Args);
                    SendMsg(string.Format("New process {0} is starting...", m.ToString()));
                    m.Process = newProcess;
                }
            }
        }
        private bool IsResetProcess(Process process)
        {
            //first init, process is null
            if (process == null)
                return true;
            //process may be exit by error or other reasons
            if (!IsProcessRunning(process))
                return true;

            //check process if it has no responding
            process.Refresh();
            if (!process.Responding)
            {
                Thread.Sleep(3000);
                if (!process.Responding)
                {
                    process.Kill();
                    SendMsg(string.Format("Process {0} has no responding, it will be killed and restart...", process.ProcessName));
                    return true;
                }
            }
            return false;
        }
        private Process CreateOneProcess(string processFile,string args)
        {
            ProcessStartInfo info = new ProcessStartInfo(processFile,args);
            //info.CreateNoWindow = true;
            Process process = Process.Start(info);           
            return process;

        }

        private void KillProcess(string processName)
        {
            processName = processName.Substring(0, processName.Length - 4);
            Process[] Processes = Process.GetProcesses();
            foreach (Process p in Processes)
            {
                if (p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    p.Kill();
                    SendMsg(string.Format("Process {0} was killed...", p.ProcessName));
                }
            }
        }

        private bool IsProcessRunning(Process process)
        {
            Process[] Processes = Process.GetProcesses();
            foreach (Process p in Processes)
            {
                if (p.Id.Equals(process.Id) && p.ProcessName.Equals (process.ProcessName))
                {
                    return true;
                }
            }
            return false;
        }
        private void SendMsg(string msg)
        {
            AppGlobal.Instance.Logger.LogDebug(msg);
            Console.WriteLine(msg);
        }

    }
}
