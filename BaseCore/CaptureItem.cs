using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public string CreateUploadStr(string header)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}@@@",header));

            sb.Append(string.Format("{0}@@@",captureTime));
            sb.Append(string.Format("{0}@@@",captureType));
            sb.Append(string.Format("{0}@@@",SourceAddress));
            sb.Append(string.Format("{0}@@@",SourceAddress2));
            sb.Append(string.Format("{0}@@@",SourceAddress3));
            sb.Append(string.Format("{0}@@@",RequestDataType));
            sb.Append(string.Format("{0}@@@",value));
            sb.Append(string.Format("{0}",UpdateRate));

            return sb.ToString();
        }
        public static CaptureItem CreateItemFromUploadStr(string line)
        {
            string[] parts = line.Split(new string[] {"@@@"},StringSplitOptions.None);
            Debug.Assert (parts.GetLength(0)>=9);
            CaptureItem item = new CaptureItem(); 
            //parts[0] is header, ignore it
            item.captureTime = parts[1];
            item.captureType = parts[2];
            item.SourceAddress = parts[3];
            item.SourceAddress2 = parts[4];
            item.SourceAddress3 = parts[5];
            item.RequestDataType = parts[6];
            item.value = parts[7];
            item.UpdateRate = parts[8];
            return item;
        }
    }
}
