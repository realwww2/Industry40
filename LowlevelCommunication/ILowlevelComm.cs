using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LowlevelCommunication
{
    public interface ILowlevelComm
    {
        void Init(string configFullFile);
        void StartRead();
        event EventHandler<CaptureItemEventArgs> OnOnceReadCompleted;
    }
}
