using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class IndexPageViewModel
    {
        public List<NewsItem> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public int MaxCommentsPerItem { get; set; }
    }
}
