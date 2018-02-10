using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class NewsItem : GenericContent
    {
        public NewsItem()
        {
            Comments = new List<NewsItemComment>();
        }
        public DateTime Published { get; set; }
        public bool IsPublished { get; set; }

        public virtual ICollection<NewsItemComment> Comments { get; set; }
        public int CommentsCount { get; set; }
    }
}
