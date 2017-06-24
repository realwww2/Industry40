using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using I4.LowlevelCommunication;
using I4.LocalCache;
using I4.RemoteServer;
using I4.LocalConfig;
using I4.BaseCore;

namespace I4.LocalCapture
{
    class Director
    {
        private Config _config;
        private ILowlevelComm _lowlevelComm;
        private ILocalCache _localCache;
        private IRemoteServer _remoteServer;

        public Director (Config config)
        {
            _config = config;
            string lowlevelComm_class = _config.GetKeyValue("LowlevelCommunication.Class");
            string localCache_class = _config.GetKeyValue("LocalCache.Class");
            string remoteServer_class = _config.GetKeyValue("RemoteServer.Class");

            

            _lowlevelComm = Activator.CreateInstance(Type.GetType(lowlevelComm_class + ", LowlevelCommunication")) as ILowlevelComm;
            _lowlevelComm.OnOnceReadCompleted += lowlevelComm_OnOnceReadCompleted;

            _localCache = Activator.CreateInstance(Type.GetType(localCache_class + ", LocalCache")) as ILocalCache;
            _remoteServer = Activator.CreateInstance(Type.GetType(remoteServer_class + ", RemoteServer")) as IRemoteServer;

        }       
       
        public void Init()
        {
            _lowlevelComm.Init(AppGlobal.Instance.Path2FullConfig(_config.GetKeyValue("LowlevelCommunication.Config")));
            _localCache.Init(AppGlobal.Instance.Path2FullConfig(_config.GetKeyValue("LocalCache.Config")));       
            _remoteServer.Init(AppGlobal.Instance.Path2FullConfig(_config.GetKeyValue("RemoteServer.Config")));
        }
        public void Execute()
        {
            _lowlevelComm.StartRead();
            Console.Read();
        }
        public void Close()
        {
            _localCache.Close();
        }
        //Here we must consider 函数重入, return directly if exist event executing

        private void lowlevelComm_OnOnceReadCompleted(object sender, CaptureItemEventArgs e)
        {
            lock(this)
            {
                try
                {
                    WriteForOnceRead(e);
                }
                catch (Exception ex)
                {
                    AppGlobal.Instance.Logger.LogError(ex);
                }                
            }

        }
        private void WriteForOnceRead(CaptureItemEventArgs e)
        {
            bool directWrite = Convert.ToBoolean(_config.GetKeyValue("RemoteServer.DirectWrite"));
            if (directWrite)
            {
                RemoteServerWriteResult remoteWriteResult;
                try
                {
                    remoteWriteResult = _remoteServer.Write(e.CaptureItems);
                }
                catch (Exception ex)
                {
                    AppGlobal.Instance.Logger.LogError(ex);
                    remoteWriteResult = RemoteServerWriteResult.FailedByException;
                }
                if (remoteWriteResult != RemoteServerWriteResult.Success)
                {
                    _localCache.Write(e.CaptureItems);
                }
            }
            else
            {
                _localCache.Write(e.CaptureItems);
            }
        }

    }
}
