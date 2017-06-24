using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using I4.BaseCore;

namespace I4.RemoteServer
{
    class TestServerImp : IRemoteServer
    {
        private string _testFile;

        public void Init(string configFullName)
        {
            _testFile = string.Format("{0}\\CacheFiles\\remoteServerDB.txt", AppDomain.CurrentDomain.BaseDirectory);
        }

        public RemoteServerWriteResult Write(CaptureItem[] items)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_testFile, true))
                {
                    foreach (CaptureItem item in items)
                    {
                        sw.WriteLine(item.CreateUploadStr("test header"));
                    }

                }
            }
            catch (Exception ex)
            {
                throw new RemoteServerException(ex.Message, ex);
            }
            if (AppDomain.CurrentDomain.FriendlyName.ToLower().Contains("localcapture"))
                return RemoteServerWriteResult.Failed;
            return RemoteServerWriteResult.Success;

        }
    }
}