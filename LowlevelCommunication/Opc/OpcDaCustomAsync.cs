using System;
using System.Collections.Generic;
using OpcRcw.Comn;
using OpcRcw.Da;
using System.Runtime.InteropServices;

namespace I4.LowlevelCommunication
{
    /// <summary>
    /// Opc自定义接口-异步管理类
    /// <author name="lm" date="2012.3.14"/>
    /// </summary>
    public class OpcDaCustomAsync : IOPCDataCallback,IDisposable
    {
        /// <summary>
        /// OPC服务器对象
        /// </summary>
        private IOPCServer _iOpcServer;
        /// <summary>
        /// 事务ID
        /// </summary>
        private int _transactionID = 0;
        /// <summary>
        /// OPC服务器名称
        /// </summary>
        private string _opcServerName;
        /// <summary>
        /// OPC服务器IP地址
        /// </summary>
        private IOPCAsyncIO2 _iopcAsyncIo2;
        /// <summary>
        /// OPC服务器IP地址
        /// </summary>
        private string _opcServerIPAddress;
        /// <summary>
        /// Opc组列表
        /// </summary>
        private List<OpcDaCustomGroup> _opcDaCustomGroups;
        /// <summary>
        /// 连接指针容器
        /// </summary>
        private IConnectionPointContainer _IConnectionPointContainer = null;
        /// <summary>
        /// 连接指针
        /// </summary>
        private IConnectionPoint _IConnectionPoint = null;
        /// <summary>
        /// Opc组管理器
        /// </summary>
        private IOPCGroupStateMgt _IOPCGroupStateMgt = null;


        //接收数据事件
        public event EventHandler<OpcDaCustomAsyncEventArgs> OnDataChanged;
        /// <summary>
        /// 异步写入数据完成事件
        /// </summary>
        public event EventHandler<OpcDaCustomAsyncEventArgs> OnWriteCompleted;
        /// <summary>
        /// 异步读取数据完成事件
        /// </summary>
        public event EventHandler<OpcDaCustomAsyncEventArgs> OnReadCompleted;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="opcDaCustomGroups">Opc组列表</param>
        /// <param name="opcServerName">OPC服务器名称</param>
        /// <param name="opcServerIpAddress">OPC服务器IP地址</param>
        public OpcDaCustomAsync(List<OpcDaCustomGroup> opcDaCustomGroups, string opcServerName, string opcServerIpAddress)
        {
            this._opcDaCustomGroups = opcDaCustomGroups;
            this._opcServerName = opcServerName;
            this._opcServerIPAddress = opcServerIpAddress;
            Init();
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        private void Init()
        {
            if (Connect())
            {
                AddOpcGroup();
            }
        }

        /// <summary>
        /// 连接Opc服务器
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return Connect(_opcServerName, _opcServerIPAddress);
        }
        /// <summary>
        /// 连接Opc服务器
        /// </summary>
        /// <returns></returns>
        public bool Connect(string remoteOpcServerName, string remoteOpcServerIpAddress)
        {
            var returnValue = false;
            if (!string.IsNullOrEmpty(remoteOpcServerIpAddress) && !string.IsNullOrEmpty(remoteOpcServerName))
            {
                var opcServerType = Type.GetTypeFromProgID(remoteOpcServerName, remoteOpcServerIpAddress);
                if (opcServerType != null)
                {
                    _iOpcServer = (IOPCServer)Activator.CreateInstance(opcServerType);
                    returnValue = true;
                }
            }  
            return returnValue;
        }
        /// <summary>
        /// 添加Opc组
        /// </summary>
        private void AddOpcGroup()
        {
            try
            {
                foreach (OpcDaCustomGroup opcGroup in _opcDaCustomGroups)
                {
                    AddOpcGroup(opcGroup);
                }
            }
            catch(COMException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 添加Opc项
        /// </summary>
        /// <param name="opcGroup"></param>
        private void AddOpcGroup(OpcDaCustomGroup opcGroup)
        {
            try
            {

                //添加OPC组
                _iOpcServer.AddGroup(opcGroup.GroupName, opcGroup.IsActive, opcGroup.RequestedUpdateRate, opcGroup.ClientGroupHandle, opcGroup.TimeBias.AddrOfPinnedObject(), opcGroup.PercendDeadBand.AddrOfPinnedObject(), opcGroup.LCID, out opcGroup.ServerGroupHandle, out opcGroup.RevisedUpdateRate, ref opcGroup.Riid, out opcGroup.Group);
                InitIoInterfaces(opcGroup);
                if (opcGroup.OpcDataCustomItems.Length > 0)
                {
                    //添加OPC项
                    AddOpcItem(opcGroup);
                    //激活订阅回调事件
                    ActiveDataChanged(_IOPCGroupStateMgt);
                }
            }
            catch (COMException ex)
            {
                throw ex;
            }
            finally
            {
                if (opcGroup.TimeBias.IsAllocated)
                {
                    opcGroup.TimeBias.Free();
                }
                if (opcGroup.PercendDeadBand.IsAllocated)
                {
                    opcGroup.PercendDeadBand.Free();
                }
            }
        }
        /// <summary>
        /// 初始化IO接口
        /// </summary>
        /// <param name="opcGroup"></param>
        public void InitIoInterfaces(OpcDaCustomGroup opcGroup)
        {
            int cookie;
            //组状态管理对象，改变组的刷新率和激活状态
            _IOPCGroupStateMgt = (IOPCGroupStateMgt)opcGroup.Group;
            _IConnectionPointContainer = (IConnectionPointContainer)opcGroup.Group;
            Guid iid = typeof(IOPCDataCallback).GUID;
            _IConnectionPointContainer.FindConnectionPoint(ref iid, out _IConnectionPoint);
            //创建客户端与服务端之间的连接
            _IConnectionPoint.Advise(this, out 
                    cookie);
        }
        /// <summary>
        /// 激活订阅回调事件
        /// </summary>
        private void ActiveDataChanged(IOPCGroupStateMgt IOPCGroupStateMgt)
        {
            IntPtr pRequestedUpdateRate = IntPtr.Zero;
            IntPtr hClientGroup = IntPtr.Zero;
            IntPtr pTimeBias = IntPtr.Zero;
            IntPtr pDeadband = IntPtr.Zero;
            IntPtr pLCID = IntPtr.Zero;
            int nActive = 0;
            GCHandle hActive = GCHandle.Alloc(nActive, GCHandleType.Pinned);
            try
            {
                hActive.Target = 1;
                int nRevUpdateRate = 0;
                IOPCGroupStateMgt.SetState(pRequestedUpdateRate, out nRevUpdateRate,
                        hActive.AddrOfPinnedObject(), pTimeBias, pDeadband, pLCID, hClientGroup);
            }
            catch (COMException ex)
            {
                throw ex;
            }
            finally
            {
                hActive.Free();
            }
        }

        /// <summary>
        /// 添加Opc项
        /// </summary>
        /// <param name="opcGroup"></param>
        private void AddOpcItem(OpcDaCustomGroup opcGroup)
        {
            OpcDaCustomItem[] opcDataCustomItemsService = opcGroup.OpcDataCustomItems;
            IntPtr pResults = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            OPCITEMDEF[] itemDefyArray = new OPCITEMDEF[opcGroup.OpcDataCustomItems.Length];
            int i = 0;
            int[] errors = new int[opcGroup.OpcDataCustomItems.Length];
            int[] itemServerHandle = new int[opcGroup.OpcDataCustomItems.Length];
            try
            {
                foreach (OpcDaCustomItem itemService in opcDataCustomItemsService)
                {
                    if (itemService != null)
                    {
                        itemDefyArray[i].szAccessPath = itemService.AccessPath;
                        itemDefyArray[i].szItemID = itemService.ItemID;
                        itemDefyArray[i].bActive = itemService.IsActive;
                        itemDefyArray[i].hClient = itemService.ClientHandle;
                        itemDefyArray[i].dwBlobSize = itemService.BlobSize;
                        itemDefyArray[i].pBlob = itemService.Blob;
                        itemDefyArray[i].vtRequestedDataType = itemService.RequestedDataType;
                        i++;
                    }

                }
                //添加OPC项组
                ((IOPCItemMgt)opcGroup.Group).AddItems(opcGroup.OpcDataCustomItems.Length, itemDefyArray, out pResults, out pErrors);
                IntPtr Pos = pResults;
                Marshal.Copy(pErrors, errors, 0, opcGroup.OpcDataCustomItems.Length);
                for (int j = 0; j < opcGroup.OpcDataCustomItems.Length; j++)
                {
                    if (errors[j] == 0)
                    {
                        if (j != 0)
                        {
                            Pos = new IntPtr(Pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMRESULT)));
                        }
                        var result = (OPCITEMRESULT)Marshal.PtrToStructure(Pos, typeof(OPCITEMRESULT));
                        itemServerHandle[j] = opcDataCustomItemsService[j].ServerHandle = result.hServer;
                        Marshal.DestroyStructure(Pos, typeof(OPCITEMRESULT));
                    }
                }
            }
            catch (COMException ex)
            {
                throw ex;
            }
            finally
            {
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                }
            }
        }
        /// <summary>
        /// 异步读取信息
        /// </summary>
        public void Read()
        {
            foreach (OpcDaCustomGroup opcGroup in _opcDaCustomGroups)
            {
                IntPtr pErrors = IntPtr.Zero;
                try
                {
                    _iopcAsyncIo2 = (IOPCAsyncIO2)opcGroup.Group;
                    if (_iopcAsyncIo2 != null)
                    {
                        int[] serverHandle = new int[opcGroup.OpcDataCustomItems.Length];
                        opcGroup.PErrors = new int[opcGroup.OpcDataCustomItems.Length];
                        for (int j = 0; j < opcGroup.OpcDataCustomItems.Length; j++)
                        {
                            serverHandle[j] = opcGroup.OpcDataCustomItems[j].ServerHandle;
                        }
                        int cancelId=0;
                        _iopcAsyncIo2.Read(opcGroup.OpcDataCustomItems.Length, serverHandle, 2, out cancelId, out pErrors);
                        Marshal.Copy(pErrors, opcGroup.PErrors, 0, opcGroup.OpcDataCustomItems.Length);
                    }
                }
                catch (COMException ex)
                {
                    throw ex;
                }
                finally
                {
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                    }
                }
            }
        }

        /// <summary>
        /// 异步写入数据
        /// </summary>
        /// <param name="values">要写入的值</param>
        /// <param name="serverHandle">要写入的项的服务器句柄</param>
        /// <param name="errors">错误信息，等于表示写入成功，否则写入失败</param>
        /// <param name="opcGroup">要写入的Opc组</param>
        public void Write(object[] values,int[] serverHandle,out int[] errors,OpcDaCustomGroup opcGroup)
        {
            _iopcAsyncIo2 = (IOPCAsyncIO2)opcGroup.Group;
            IntPtr pErrors = IntPtr.Zero;
            errors = new int[values.Length];
            if (_iopcAsyncIo2 != null)
            {
                try
                {
                    //异步写入数据
                    int cancelId;
                    _iopcAsyncIo2.Write(values.Length, serverHandle, values, _transactionID + 1, out cancelId, out pErrors);
                    Marshal.Copy(pErrors, errors, 0, values.Length);
                }
                catch (COMException ex)
                {
                    throw ex;
                }
                finally
                {
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                    }
                }
            }
        }
        /// <summary>
        /// 数据订阅事件
        /// </summary>
        /// <param name="dwTransid"></param>
        /// <param name="hGroup"></param>
        /// <param name="hrMasterquality"></param>
        /// <param name="hrMastererror"></param>
        /// <param name="dwCount"></param>
        /// <param name="phClientItems"></param>
        /// <param name="pvValues"></param>
        /// <param name="pwQualities"></param>
        /// <param name="pftTimeStamps"></param>
        /// <param name="pErrors"></param>
        public virtual void OnDataChange(Int32 dwTransid,
            Int32 hGroup,
            Int32 hrMasterquality,
            Int32 hrMastererror,
            Int32 dwCount,
            int[] phClientItems,
            object[] pvValues,
            short[] pwQualities,
            OpcRcw.Da.FILETIME[] pftTimeStamps,
            int[] pErrors)
        
        {
            var e = new OpcDaCustomAsyncEventArgs
            {
                GroupHandle = hGroup,
                Count = dwCount,
                Errors = pErrors,
                Values = pvValues,
                ClientItemsHandle = phClientItems
            };
            if (OnDataChanged != null)
            {
                OnDataChanged(this, e);
            }
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        /// <param name="dwTransid"></param>
        /// <param name="hGroup"></param>
        public virtual void OnCancelComplete(Int32 dwTransid, Int32 hGroup)
        {

        }

        /// <summary>
        /// 写入数据完成事件
        /// </summary>
        /// <param name="dwTransid"></param>
        /// <param name="hGroup"></param>
        /// <param name="hrMastererr"></param>
        /// <param name="dwCount"></param>
        /// <param name="pClienthandles"></param>
        /// <param name="pErrors"></param>
        public virtual void OnWriteComplete(Int32 dwTransid,
            Int32 hGroup,
            Int32 hrMastererr,
            Int32 dwCount,
            int[] pClienthandles,
            int[] pErrors)
        {
            if (OnWriteCompleted != null)
            {
                var e = new OpcDaCustomAsyncEventArgs
                {
                    Errors = pErrors
                };
                if (OnWriteCompleted != null)
                {
                    OnWriteCompleted(this, e);
                }
            }
        }
        /// <summary>
        /// 读取数据完成事件
        /// </summary>
        /// <param name="dwTransid"></param>
        /// <param name="hGroup"></param>
        /// <param name="hrMasterquality"></param>
        /// <param name="hrMastererror"></param>
        /// <param name="dwCount">要读取的组的项的个数</param>
        /// <param name="phClientItems"></param>
        /// <param name="pvValues">项值列表</param>
        /// <param name="pwQualities"></param>
        /// <param name="pftTimeStamps"></param>
        /// <param name="pErrors">项错误列表</param>
        public virtual void OnReadComplete(Int32 dwTransid,
            Int32 hGroup,
            Int32 hrMasterquality,
            Int32 hrMastererror,
            Int32 dwCount,
            int[] phClientItems,
            object[] pvValues,
            short[] pwQualities,
            OpcRcw.Da.FILETIME[] pftTimeStamps,
            int[] pErrors)
        {
            if (OnReadCompleted != null)
            {
                var e = new OpcDaCustomAsyncEventArgs
                {
                    GroupHandle = hGroup,
                    Count = dwCount,
                    Errors = pErrors,
                    Values = pvValues,
                    ClientItemsHandle = phClientItems
                };
                OnReadCompleted(this, e);
            }
        }
        public void Dispose()
        {

        }
    }
}
