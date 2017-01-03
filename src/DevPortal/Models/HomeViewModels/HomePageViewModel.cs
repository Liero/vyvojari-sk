using DevPortal.Web.Models.ForumViewModels;
using DevPortal.Web.Models.NewsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.HomeViewModels
{
    public class HomePageViewModel
    {
        public List<NewsItemViewModel> LatestNews { get; set; }
        public List<ForumPostViewModel> LatestPosts { get; set; }
    }
}
