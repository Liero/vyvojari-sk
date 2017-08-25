using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumThreadDeleted : DomainEvent
    {
        public string UserName { get; set; }
        public Guid ForumThreadId { get; set; }
    }
}
