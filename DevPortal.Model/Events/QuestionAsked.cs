using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.Model.Events
{
    public class QuestionAsked : Event
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
    }
}
