using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;
using I4.LocalConfig;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;
using System.IO;

namespace I4.LocalCache
{
    public sealed class MongoDBSimulatorCacheImp:ILocalCache
    {
        private string _cachePath;
        private string _cacheCount
        {
            get { return DateTime.Now.Ticks.ToString(); }
        }
        private const string WORKPREFIX = "working";
        private const string NOTWORKPREFIX = "notworking";
        private string GetWorkFile()
        {
            return string.Format("{0}\\{1}_{2}.cache", _cachePath, WORKPREFIX, _cacheCount);
        }
        private string notWorkFile
        {
            get { return string.Format("{0}\\{1}_{2}.cache", _cachePath, NOTWORKPREFIX, _cacheCount); }
        }
        public void Init(string configFullFile)
        {
            _cachePath = string.Format("{0}\\CacheFiles", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists(_cachePath))
                Directory.CreateDirectory(_cachePath);
        }

        //write items into 1 file
        public void Write(CaptureItem[] items)
        {
            string workFile = GetWorkFile();
            using (StreamWriter sw = new StreamWriter(workFile))
            {
                foreach (CaptureItem item in items)
                    sw.WriteLine(item.CreateUploadStr("test header"));
            }
            File.Move(workFile, workFile.Replace(WORKPREFIX, NOTWORKPREFIX));
        }
        //read from one file
        public IList<CaptureItem> ReadOneBatch(ref string batchName)
        {
            IList<CaptureItem> items = new List<CaptureItem>();
            while (true)
            {
                Thread.Sleep(1000);
                string[] files = Directory.GetFiles(_cachePath, NOTWORKPREFIX + "*");
                if (files.GetLength(0) == 0) continue;

                using (StreamReader sr = new StreamReader(files[0]))
                {
                    for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
                    {
                        items.Add(CaptureItem.CreateItemFromUploadStr (line));
                    }
                    batchName = files[0];
                    return items;
                }
            }
        }
        //delete one file
        public void Delete(string batchName)
        {
            File.Delete(batchName);
        }
        public void Close()
        {
           
        }
    }
}