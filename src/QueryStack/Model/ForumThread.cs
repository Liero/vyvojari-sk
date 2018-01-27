using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumThread
    {
        public ForumThread()
        {
            Posts = new HashSet<ForumPost>();
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Time when thread was created or last post was addedd
        /// </summary>
        public DateTime LastPosted { get; set; }
        public string LastPostedBy { get; set; }
        public virtual ICollection<ForumPost> Posts { get; set; }
        public int PostsCount { get; set; }
    }   
}
