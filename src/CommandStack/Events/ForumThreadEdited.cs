using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ForumThreadEdited : DomainEvent, IContentEdited
    {
        public Guid ForumThreadId { get; set; }
        public string EditorUserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }

        public string Reason { get; set; }

        Guid IContentEvt.ContentId => ForumThreadId;
    }
}
