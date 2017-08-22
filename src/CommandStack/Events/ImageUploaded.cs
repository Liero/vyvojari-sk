using DevPortal.CommandStack.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Events
{
    public class ImageUploaded : DomainEvent
    {
        public string UserName { get; set; }
        public string ImageId { get; set; }
        public string System { get; set; }
        public string Link { get; set; }
    }
}
