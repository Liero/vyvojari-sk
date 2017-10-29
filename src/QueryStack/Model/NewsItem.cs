using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class NewsItem
    {
        public NewsItem()
        {
            Comments = new HashSet<NewsItemComment>();
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime Published { get; set; }
        public bool IsPublished { get; set; }

        public virtual ICollection<NewsItemComment> Comments { get; set; }
        public int CommentsCount { get; set; }
    }
}
