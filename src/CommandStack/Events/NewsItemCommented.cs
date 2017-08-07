using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class NewsItemCommented : DomainEvent
    {
        public Guid NewsItemId { get; set; }
        public Guid CommentId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
    }
}
