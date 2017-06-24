using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalCache
{
    class LocalCacheException : Exception
    {
        public LocalCacheException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
