using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumPostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime Created { get; internal set; }
    }
}
