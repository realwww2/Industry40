using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalSchedule
{
    class LocalScheduleException:Exception
    {
        public LocalScheduleException(string message, Exception innerException = null)
            : base(message,innerException)
        {

        }
    }
}
