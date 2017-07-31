using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.Model.Entities.Forum
{
    public class ForumQuestion : ForumPost
    {
        public string Title { get; set; }
        public List<string> Tags { get; set; }
    }
}
