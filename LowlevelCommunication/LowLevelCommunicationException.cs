﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LowlevelCommunication
{
    class LowLevelCommunicationException:Exception
    {
        public LowLevelCommunicationException(string message, Exception innerException = null)
            : base(message,innerException)
        {

        }
    }
}
