using DevPortal.QueryStack.Model;
using DevPortal.Web.Models.SharedViewModels;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.SearchViewModels
{
    public class IndexPageViewModel : PaginationViewModelBase
    {
        public List<SearchItem> Items { get; set; } = new List<SearchItem>();
    }

    public class SearchItem
    {
        public Guid Id { get; set; }
        public Guid Fragment { get; set; }
        public string Title { get; set; }
        public Type ContentType { get; set; }
        public string[] Tags { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }
}
