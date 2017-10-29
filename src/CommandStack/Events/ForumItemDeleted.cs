using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumItemDeleted : DomainEvent
    {
        public Guid ForumThreadId { get; set; }
        public Guid ForumItemId { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
    }
}
