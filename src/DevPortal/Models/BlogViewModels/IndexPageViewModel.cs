using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class IndexPageViewModel
    {
        public List<Blog> Blogs { get; internal set; }
        public int PageNumber { get; internal set; }

        public IndexPageViewModel()
        {

        }
    }
}
