using DevPortal.QueryStack.Model;
using DevPortal.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class IndexPageViewModel : PaginationViewModelBase
    {
        public List<ForumThread> Threads { get; set; }

        public string[] MostUsedTags { get; set; }

        public IndexPageViewModel()
        {
            MostUsedTags = new[] { "" };
        }
    }

}
