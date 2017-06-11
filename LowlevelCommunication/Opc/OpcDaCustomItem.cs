using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpcRcw.Da;

namespace I4.LowlevelCommunication
{
    /// <summary>
    /// 自定义接口Opc项
    /// </summary>
    public class OpcDaCustomItem
    {
        private string _name;
        private string _accessPath="";
        private string _itemID;
        private int _isActive = 1;
        private int _clientHandle = 0;
        private int _blobSize = 0;
        private IntPtr _blob = IntPtr.Zero;
        private short _requestedDataType = 0;
        private object _itemValue;
        private int _serverHandle;

        /// <summary>
        /// 项名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;
                _name = value;
            }
        }
        /// <summary>
        /// 项对象的访问路径
        /// </summary>
        public string AccessPath
        {
            get
            {
                return _accessPath;
            }
            set
            {
                if (_accessPath == value)
                    return;
                _accessPath = value;
            }
        }

        /// <summary>
        /// 项对象的ItemIDea，唯一标识该数据项
        /// </summary>
        public string ItemID
        {
            get
            {
                return _itemID;
            }
            set
            {
                if (_itemID == value)
                    return;
                _itemID = value;
            }
        }

        /// <summary>
        /// 项对象的激活状态
        /// 1为激活，0为未激活，默认激活
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
        /// 项对象的客户端句柄
        /// </summary>
        public int ClientHandle
        {
            get
            {
                return _clientHandle;
            }
            set
            {
                if (_clientHandle == value)
                    return;
                _clientHandle = value;
            }
        }
        public int BlobSize
        {
            get
            {
                return _blobSize;
            }
            set
            {
                if (_blobSize == value)
                    return;
                _blobSize = value;
            }
        }
        public IntPtr Blob
        {
            get
            {
                return _blob;
            }
            set
            {
                if (_blob == value)
                    return;
                _blob = value;
            }
        }

        /// <summary>
        /// OPC项的数据类型
        /// VbBoolean:11，VbByte:17,VbDecimal:14,VbDouble:5,Vbinteger:2,VbLong:3,VbSingle:4,VbString:8
        /// </summary>
        public short RequestedDataType
        {
            get
            {
                return _requestedDataType;
            }
            set
            {
                if (_requestedDataType == value)
                    return;
                _requestedDataType = value;
            }
        }

       /// <summary>
       /// OPC项的值
       /// </summary>
        public object Value
        {
            get
            {
                return _itemValue;
            }
            set
            {
                if (_itemValue == value)
                    return;
                _itemValue = value;
            }
        }

        /// <summary>
        /// OPC项的服务器句柄
        /// </summary>
        public int ServerHandle
        {
            get
            {
                return _serverHandle;
            }
            set
            {
                if (_serverHandle == value)
                    return;
                _serverHandle = value;
            }
        }
    }
}
