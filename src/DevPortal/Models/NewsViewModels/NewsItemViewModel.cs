using DevPortal.Web.Models.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class NewsItemViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string Categories { get; set; }

        public string UserName { get; set; }

        public bool IsPublished { get; set; }

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
