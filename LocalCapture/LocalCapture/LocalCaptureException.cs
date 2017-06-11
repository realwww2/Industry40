using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalCapture
{
    class LocalCaptureException:Exception
    {
        public LocalCaptureException(string message, Exception innerException = null)
            : base(message,innerException)
        {

        }
    }
}
