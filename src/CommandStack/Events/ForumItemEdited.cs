using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumItemEdited : DomainEvent
    {
        public Guid ForumThreadId { get; set; }
        public Guid ForumItemId { get; set; }
        public string EditorUserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
    }
}
