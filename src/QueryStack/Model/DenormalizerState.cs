using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class DenormalizerState
    {
        public string Key { get; set; }
        public long EventNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
