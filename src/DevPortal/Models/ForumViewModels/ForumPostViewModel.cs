using System;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumPostViewModel
    {
        public string Content { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime Created { get; set; }
    }
}
