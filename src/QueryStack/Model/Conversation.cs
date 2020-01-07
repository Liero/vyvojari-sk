using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    /// <summary>
    /// represents conversation between two participants
    /// </summary>
    public class Conversation
    {
        public string UserName1 { get; set; }
        public string UserName2 { get; set; }
        public string LastContent { get; set; }
        public string LastPostedBy { get; set; }
        public DateTime LastPosted { get; set; }
        public int UnreadMessages { get; set; }
    }
}
