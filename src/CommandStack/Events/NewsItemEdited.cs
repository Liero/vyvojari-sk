﻿using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class NewsItemEdited : DomainEvent, IContentEdited
    {
        public Guid NewsItemId { get; set; }
        public string EditorUserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }

        Guid IContentEvt.ContentId => NewsItemId;
    }
}
