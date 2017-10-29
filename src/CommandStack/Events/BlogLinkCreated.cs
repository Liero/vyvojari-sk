using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class BlogLinkCreated : DomainEvent
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
    }
}
