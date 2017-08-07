using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class NewsItemPublished : DomainEvent
    {
        public Guid NewsItemId { get; set; }
    }
}
