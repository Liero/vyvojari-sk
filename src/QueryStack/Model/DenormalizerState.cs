using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class DenormalizerState
    {
        public string TypeName { get; set; }
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
