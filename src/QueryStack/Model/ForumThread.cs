using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumThread : GenericContent
    {
        public ForumThread()
        {
            Posts = new List<ForumPost>();
        }
     
        /// <summary>
        /// Time when thread was created or last post was addedd
        /// </summary>
        public DateTime LastPosted { get; set; }
        public string LastPostedBy { get; set; }
        public virtual ICollection<ForumPost> Posts { get; set; }
        public int PostsCount { get; set; }

        public string ParticipantsCsv { get; set; } = "";
    }   
}
