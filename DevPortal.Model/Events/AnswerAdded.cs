using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.Model.Events
{
    public class AnswerAdded : Event
    {
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}
