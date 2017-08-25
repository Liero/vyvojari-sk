using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumItemPosted : DomainEvent
    {
        public Guid ForumThreadId { get; set; }
        public Guid ForumItemId { get; set; }
        public string AuthorUserName { get; set; }
        public string Content { get; set; }
    }
}
