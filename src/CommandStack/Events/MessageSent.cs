using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class MessageSent : DomainEvent
    {
        public Guid MessageId { get; set; }
        public string SenderUserName { get; set; }
        public string RecipientUserName { get; set; }
        public string Content { get; set; }
    }
}
