using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumThreadCreated : DomainEvent
    {
        public Guid ForumThreadId { get; set; }
        public string AuthorUserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
    }
}
