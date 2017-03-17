using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.Model.Events
{
    public abstract class Event
    {
        public string AggregateId { get; set; }
        public string AggregateName { get; set; }
        public DateTime Date { get; set; }
    }
}
