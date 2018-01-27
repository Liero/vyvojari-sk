using DevPortal.QueryStack.Model;
using DevPortal.Web.Models.SharedViewModels;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class IndexPageViewModel : PaginationViewModelBase
    {
        public List<NewsItem> Items { get; set; }
        public int MaxCommentsPerItem { get; set; }

        public AddCommentViewModel AddComment { get; set; }

        public CommentViewModel CreateCommentVm(NewsItemComment comment)
        {
            return new CommentViewModel
            {
                Id = comment.Id,
                Message = comment.Content.Truncate(200),
                Created = comment.Created,
                UserName = comment.CreatedBy,
            };
        }
    }
}
