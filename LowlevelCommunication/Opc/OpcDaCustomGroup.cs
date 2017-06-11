using System;
using System.Runtime.InteropServices;
using OpcRcw.Da;

namespace I4.LowlevelCommunication
{
    /// <summary>
    /// 自定义接口OPC组对象
    /// </summary>
    public class OpcDaCustomGroup
    {
        private string _groupName;
        private int _isActive=1;
        private int _requestedUpdateRate;
        private int _clientGroupHandle=1;
        private GCHandle _timeBias = GCHandle.Alloc(0, GCHandleType.Pinned);
        private GCHandle _percendDeadBand = GCHandle.Alloc(0, GCHandleType.Pinned);
        private int _lcid = 0x409;
        private int _itemCount;
        private bool _onRead;

        /// <summary>
        /// 输出参数,服务器为新创建的组对象产生的句柄
        /// </summary>
        public int ServerGroupHandle;

        /// <summary>
        /// 输出参数，服务器返回给客户端的实际使用的数据更新率
        /// </summary>
        public int RevisedUpdateRate;

        /// <summary>
        /// 引用参数，客户端想要的组对象的接口类型(如 IIDIOPCItemMgt)
        /// </summary>
        public Guid Riid = typeof(IOPCItemMgt).GUID;

        /// <summary>
        /// 输出参数，用来存储返回的接口指针。如果函数操作出现任务失败，此参数将返回NULL。
        /// </summary>
        public object Group;
        private OpcDaCustomItem[] opcDataCustomItems;

        public int[] PErrors { get; set; }

        /// <summary>
        /// 组对象是否激活
        /// 1为激活，0为未激活,默认激活
        /// </summary>
        public int IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive == value)
                    return;
                _isActive = value;
            }
        }
        /// <summary>
        /// 组是否采用异步读方式
        /// </summary>
        public bool OnRead
        {
            get
            {
                return _onRead;
            }
            set
            {
                if (_onRead == value)
                    return;
                _onRead = value;
            }
        }
        /// <summary>
        /// 项的个数
        /// </summary>
        public int ItemCount
        {
            get { return _itemCount; }
            set 
            {
                if(_itemCount == value)
                    return;
                _itemCount=value;
            }
        }
        /// <summary>
        /// 客户端指定的数据变化率
        /// </summary>
        public int RequestedUpdateRate
        {
            get
            {
                return _requestedUpdateRate;
            }
            set
            {
                if (_requestedUpdateRate == value)
                    return;
                _requestedUpdateRate = value;
            }
        }

        /// <summary>
        /// OPC组名称
        /// </summary>
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName == value)
                    return;
                _groupName = value;
            }
        }

        /// <summary>
        /// 客户端程序为组对象提供的句柄
        /// </summary>
        public int ClientGroupHandle
        {
            get
            {
                return _clientGroupHandle;
            }
            set
            {
                if (_clientGroupHandle == value)
                    return;
                _clientGroupHandle = value;
            }
        }

        /// <summary>
        /// 指向Long类型的指针
        /// </summary>
        public GCHandle TimeBias
        {
            get
            {
                return _timeBias;
            }
            set
            {
                if (_timeBias == value)
                    return;
                _timeBias = value;
            }
        }

        /// <summary>
        /// 一个项对象的值变化的百分比，可能引发客户端程序的订阅回调。
        /// 此参数只应用于组对象中有模拟dwEUType(工程单位)类型的项对象。指针为NULL表示0.0
        /// </summary>
        public GCHandle PercendDeadBand
        {
            get
            {
                return _percendDeadBand;
            }
            set
            {
                if (_percendDeadBand == value)
                    return;
                _percendDeadBand = value;
            }
        }

        /// <summary>
        /// 当用于组对象上的操作的返回值为文本类型时，服务器使用的语言
        /// </summary>
        public int LCID
        {
            get
            {
                return _lcid;
            }
            set
            {
                if (_lcid == value)
                    return;
                _lcid = value;
            }
        }

        /// <summary>
        /// OPC项数组
        /// </summary>
        public OpcDaCustomItem[] OpcDataCustomItems
        {
            get
            {
                return opcDataCustomItems;
            }
            set
            {
                if (opcDataCustomItems != null && opcDataCustomItems == value)
                    return;
                opcDataCustomItems = value;
            }
        }
    }
}
