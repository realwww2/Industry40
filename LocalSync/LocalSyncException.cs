﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I4.LocalSync
{
    class LocalSyncException:Exception
    {
        public LocalSyncException(string message, Exception innerException = null)
            : base(message,innerException)
        {

        }
    }
}
