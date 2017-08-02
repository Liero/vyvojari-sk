using DevPortal.CommandStack.Infrastructure;
using System;

namespace DevPortal.CommandStack.Events
{
    public class NewsItemCreated : DomainEvent
    {
        public Guid NewsItemId { get; set; }
        public string AuthorUserName { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
    }
}
