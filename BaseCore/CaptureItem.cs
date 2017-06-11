using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.BaseCore
{
    public class CaptureType
    {
        public const string Opc = "0";
        public const string OpcSimulator = "10";
        public const string Plc = "1";
        public const string PlcSimulator = "11";
    }
    public class CaptureItem
    {
        //Capture Time: "yyyy-mm-dd hh mm ss"
        public string captureTime { get; set; }
        //Capture Type: "Opc", "Plc", "Opc Simulator", "Plc Simulator"
        public string captureType { get; set; }
        //Source Address: "OPC.SimaticNET(10.102.102.118)"
        public string SourceAddress { get; set; }
        //Source Address2: "ShearerInfoGroup"
        public string SourceAddress2 { get; set; }
        //Source Address3: "S7:[S7 connection_2]DB201,X20.2"
        public string SourceAddress3 { get; set; }
        //Request DataType: "int", "string"
        public string RequestDataType { get; set; }
        //Capture Item Value: "xxxxxxxxxx"
        public string value { get; set; }
        //update rate(ms): "500", "1000"
        public string UpdateRate { get; set; }
    }
}
