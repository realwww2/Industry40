using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalCache
{
    public interface ILocalCache
    {
        void Init(string configFullFile);
        void Write();
        void Read();
    }
}
