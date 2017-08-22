using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class AvatarChanged : DomainEvent
    {
        public string Url { get; set; }
        public string UserName { get; set; }
    }
}
