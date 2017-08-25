using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumPost
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
