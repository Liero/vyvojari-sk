using DevPortal.QueryStack.Model;
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
        public List<NewsItem> LatestNews { get; set; }
    }
}
