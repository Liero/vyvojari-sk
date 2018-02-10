using DevPortal.CommandStack.Infrastructure;
using System;

namespace DevPortal.CommandStack.Events
{
    public class NewsItemCreated : DomainEvent, IContentCreated
    {
        public Guid NewsItemId { get; set; }
        public string AuthorUserName { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }

        Guid IContentEvt.ContentId => NewsItemId;
    }

    public interface IContentEvt
    {
        Guid ContentId { get; }
        string Title { get; }
        string Content { get; }
        string[] Tags { get; }
        DateTime TimeStamp { get; }
    }

    public interface IContentCreated : IContentEvt
    {
        string AuthorUserName { get; }
    }

    public interface IContentEdited : IContentEvt
    {
        string EditorUserName { get; }
    }
}
