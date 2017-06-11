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
            string lowlevelComm_class = config.GetKeyValue("LowlevelCommunication.Class");
            string localCache_class = config.GetKeyValue("LocalCache.Class");
            string remoteServer_class = config.GetKeyValue("RemoteServer.Class");

            _lowlevelComm = Activator.CreateInstance(Type.GetType(lowlevelComm_class)) as ILowlevelComm;
            _lowlevelComm.Init(AppGlobal.Instance.Path2Full(config.GetKeyValue("LowlevelCommunication.Config")));
            _lowlevelComm.OnOnceReadCompleted += lowlevelComm_OnOnceReadCompleted;

            _localCache = Activator.CreateInstance(Type.GetType(localCache_class)) as ILocalCache ;
            _localCache.Init(AppGlobal.Instance.Path2Full(config.GetKeyValue("LocalCache.Config")));

            _remoteServer = Activator.CreateInstance(Type.GetType(remoteServer_class)) as IRemoteServer;
            _remoteServer.Init(AppGlobal.Instance.Path2Full(config.GetKeyValue("RemoteServer.Config")));
        }        
        public void Execute()
        {
            _lowlevelComm.StartRead();
        }
        //Here we must consider 函数重入, return directly if exist event executing
        private int _iLock = 0;
        private void lowlevelComm_OnOnceReadCompleted(object sender, CaptureItemEventArgs e)
        {
            System.Threading.Interlocked.Increment(ref _iLock);
            if(_iLock==0)
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
            System.Threading.Interlocked.Decrement(ref _iLock);
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
