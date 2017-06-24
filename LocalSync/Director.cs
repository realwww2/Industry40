using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using I4.LocalCache;
using I4.RemoteServer;
using I4.LocalConfig;
using I4.BaseCore;

namespace I4.LocalSync
{
    class Director
    {
        private Config _config;
        private ILocalCache _localCache;
        private IRemoteServer _remoteServer;

        public Director (Config config)
        {
            _config = config;
            string localCache_class = _config.GetKeyValue("LocalCache.Class");
            string remoteServer_class = _config.GetKeyValue("RemoteServer.Class");

            _localCache = Activator.CreateInstance(Type.GetType(localCache_class + ",LocalCache")) as ILocalCache;
            _remoteServer = Activator.CreateInstance(Type.GetType(remoteServer_class + ",RemoteServer")) as IRemoteServer;

        }       
        public void Init()
        {
            _localCache.Init(AppGlobal.Instance.Path2FullConfig(_config.GetKeyValue("LocalCache.Config")));       
            _remoteServer.Init(AppGlobal.Instance.Path2FullConfig(_config.GetKeyValue("RemoteServer.Config")));
        }
        public void Execute()
        {
            Thread thread = new Thread(this.ThreadReadCache);
            thread.Start();

        }
        private void ThreadReadCache()
        {
            while(true)
            {
                //Read one collection in db
                string batchName=string.Empty;
                IList<CaptureItem> cs;
                try
                {
                    cs = _localCache.ReadOneBatch(ref batchName);
                }
                catch (Exception ex)
                {
                    AppGlobal.Instance.Logger.LogError(ex);
                    continue;
                }

                //Upload all read datas
                RemoteServerWriteResult remoteWriteResult;
                try
                {
                    remoteWriteResult = _remoteServer.Write(cs.ToArray<CaptureItem>());
                }
                catch (Exception ex)
                {
                    AppGlobal.Instance.Logger.LogError(ex);
                    remoteWriteResult = RemoteServerWriteResult.FailedByException;
                }

                //Delete this collection, here we can't promise the delete opeartion will be successful, so the upload db may have duplicate datas
                if (remoteWriteResult == RemoteServerWriteResult.Success)
                {
                    try
                    {
                        _localCache.Delete(batchName);
                    }
                    catch (Exception ex)
                    {
                        AppGlobal.Instance.Logger.LogError(ex);
                    }
                }
            }
        }
        public void Close()
        {
            _localCache.Close();
        }
      

    }
}
