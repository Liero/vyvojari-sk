using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class MessageMarkedAsRead : DomainEvent
    {
        public Guid MessageId { get; set; }
        public string UserName { get; set; }
    }
}
