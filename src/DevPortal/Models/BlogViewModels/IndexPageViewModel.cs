using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class IndexPageViewModel: IPagination
    {
        public List<Blog> Blogs { get; set; }
        public int PageNumber { get; set; }
        public int PagesCount { get; set; }
    }
}
