using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public string RecipientUserName { get; set; }
        public string SenderUserName { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
