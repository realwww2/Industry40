using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.RemoteServer
{
    class RemoteServerException : Exception
    {
        public RemoteServerException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
