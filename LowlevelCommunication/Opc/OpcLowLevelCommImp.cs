using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using I4.BaseCore;

namespace I4.LowlevelCommunication
{

    //We must implement ILowlevelComm for each lowlevel interface
    public class OpcLowLevelCommImp : ILowlevelComm
    {
        private OpcManager _opcManager;

        public void Init(string configFullFile)
        {
            Debug.Assert(_opcManager == null);
            _opcManager = new OpcManager(configFullFile);
            _opcManager.OnReadCompleted += opcManager_OnReadCompleted;
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
        private void opcManager_OnReadCompleted(object sender, OpcDaCustomAsyncEventArgs e)
        {
            CaptureItem[] captureItems = Convert2CaptureItem(e);
            CaptureItemEventArgs ce = new CaptureItemEventArgs();
            ce.CaptureItems = captureItems;
            this.OnOnceReadCompleted(sender, ce);
        }
        private CaptureItem[] Convert2CaptureItem(OpcDaCustomAsyncEventArgs e)
        {
            CaptureItem[] items = new CaptureItem[e.Count];
            for (int i = 0; i < e.Count; i++)
            {
                items[i] = new CaptureItem();
                items[i].captureTime = DateTime.Now.ToString("yyyy-mm-dd hh mm ss");
                items[i].captureType = CaptureType.Opc;
                items[i].SourceAddress = _opcManager.SourceAddress1;

                OpcDaCustomGroup group = _opcManager.GetGroup(e.GroupHandle);
                if(group!=null) items[i].SourceAddress2 = group.GroupName;

                OpcDaCustomItem opcItem = _opcManager.GetItem(e.GroupHandle, e.ClientItemsHandle[i]);
                if(opcItem!=null) items[i].SourceAddress3 = opcItem.Name;

                if(group!=null) items[i].UpdateRate = group.RequestedUpdateRate.ToString();

                if (opcItem != null) items[i].RequestDataType = opcItem.RequestedDataType.ToString();

                if (opcItem != null) items[i].value = e.Values[i].ToString();// here we may update when debugging
            }
            return items;
        }
        public event EventHandler<CaptureItemEventArgs> OnOnceReadCompleted;

    }
}
