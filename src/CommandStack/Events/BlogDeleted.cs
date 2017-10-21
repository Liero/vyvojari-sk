using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class BlogDeleted : DomainEvent
    {
        public Guid BlogId { get; set; }
        public string UserName { get; set; }
    }
}
