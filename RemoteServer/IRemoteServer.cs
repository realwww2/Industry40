using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;

namespace I4.RemoteServer
{
    public enum RemoteServerWriteResult
    {
        Success = 0,
        Failed = 1,
        FailedByException = 2,
        DirectWriteDisable = 99
    }
    public interface IRemoteServer
    {
        void Init(string configFullName);
        RemoteServerWriteResult Write(CaptureItem[] items);

    }
}
