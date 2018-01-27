using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class IndexPageViewModel: PaginationViewModelBase
    {
        public List<Blog> Blogs { get; set; }
    }
}
