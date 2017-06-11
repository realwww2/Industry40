using System;
using I4.BaseCore;

namespace I4.LowlevelCommunication
{
    public class CaptureItemEventArgs : EventArgs
    {
        //Flat capture items
        public CaptureItem[] CaptureItems;
    }
}
