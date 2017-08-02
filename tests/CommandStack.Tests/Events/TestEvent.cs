using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class TestEvent : DomainEvent
    {
        public Guid TestEventId { get; set; }
    }
}
