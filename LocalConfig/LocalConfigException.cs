using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalConfig
{
    class LocalConfigException:Exception
    {
        public LocalConfigException(string message,Exception innerException = null)
            : base(message,innerException)
        {

        }
    }
}
