using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;

namespace I4.LowlevelCommunication
{

    //We must implement ILowlevelComm for each lowlevel interface
    public class OpcLowLevelCommImp:ILowlevelComm
    {
        private OpcManager _opcManager;

        public void Init(string configFullFile)
        {
            Debug.Assert(_opcManager == null);
            _opcManager = new OpcManager(configFullFile);
        }
        private int GetMaxUpdateRateOfGroups()
        {
            int iMax = 0;
            foreach (OpcDaCustomGroup group in _opcManager.opcGroups)
            {
                if (group.RequestedUpdateRate > iMax)
                    iMax = group.RequestedUpdateRate;
            }
            return iMax;
        }
        public void StartRead()
        {
            Timer opcTimer = new System.Timers.Timer()
            {
                Interval = GetMaxUpdateRateOfGroups(),
                AutoReset = true,
                Enabled = true
            };
            opcTimer.Elapsed += new ElapsedEventHandler(opcTimer_Elapsed);
        }
        private void opcTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _opcManager.Read();
        }
        public event EventHandler<ReadAsyncEventArgs> OnOnceReadCompleted;
    }
}
