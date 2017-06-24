using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I4.BaseCore;

namespace I4.LocalCache
{

    public interface ILocalCache
    {
        void Init(string configFullFile);
        void Close();
        void Write(CaptureItem[] items);
        IList<CaptureItem> ReadOneBatch(ref string batchName);
        void Delete(string batchName);

    }
}
