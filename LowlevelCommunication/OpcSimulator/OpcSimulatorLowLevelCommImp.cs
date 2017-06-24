using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using I4.BaseCore;
using I4.Log;

namespace I4.LowlevelCommunication
{

    //We must implement ILowlevelComm for each lowlevel interface
    public class OpcSimulatorLowLevelCommImp : ILowlevelComm
    {


        public void Init(string configFullFile)
        {
            
        }


       
        public void StartRead()
        {
            Timer opcTimer = new System.Timers.Timer()
            {
                Interval = 500,
                AutoReset = true,
                Enabled = true
            };
            opcTimer.Elapsed += new ElapsedEventHandler(opcTimer_Elapsed);
        }
        private long _totalCount = 0;
        private void opcTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer t = sender as System.Timers.Timer;
            //t.AutoReset = false;
            //t.Enabled = false;
            int batchNum = 100;
            CaptureItem[] items = FakeCaptureItem(batchNum);
            _totalCount += batchNum;
            AppGlobal.Instance.Logger.LogDebug(string.Format("Captured {0} items, total captured {1} items", batchNum, _totalCount));
            OnOnceReadCompleted(this, new CaptureItemEventArgs()
                {
                  CaptureItems = items
                }
                );
            //t.AutoReset = true;
            //t.Enabled = true;
        }

        private CaptureItem[] FakeCaptureItem(int batchNum)
        {
            CaptureItem[] items = new CaptureItem[batchNum];
            for (int i = 0; i < batchNum - 1; i++)
            {
                items[i] = new CaptureItem();
                items[i].captureTime = DateTime.Now.ToString("yyyy-mm-dd hh mm ss");
                items[i].captureType = CaptureType.OpcSimulator;
                items[i].SourceAddress = "OPC.SimaticNET";
                items[i].SourceAddress2 = "ShearerInfoGroup";
                items[i].SourceAddress3 = "S7:[S7 connection_2]DB201,X20.2";
                items[i].UpdateRate = "500";
                items[i].RequestDataType = "11";
                items[i].value = "1";
                i++;
                items[i] = new CaptureItem();
                items[i].captureTime = DateTime.Now.ToString("yyyy-mm-dd hh mm ss");
                items[i].captureType = CaptureType.OpcSimulator;
                items[i].SourceAddress = "OPC.SimaticNET";
                items[i].SourceAddress2 = "ShearerInfoGroup";
                items[i].SourceAddress3 = "S7:[S7 connection_2]DB201,REAL40";
                items[i].UpdateRate = "500";
                items[i].RequestDataType = "5";
                items[i].value = i.ToString();
            }
            return items;
        }
        public event EventHandler<CaptureItemEventArgs> OnOnceReadCompleted;

    }
}
