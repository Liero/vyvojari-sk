using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumThread
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public virtual List<ForumPost> Posts { get; set; }
        public int AnswersCount { get; set; }
    }   
}
