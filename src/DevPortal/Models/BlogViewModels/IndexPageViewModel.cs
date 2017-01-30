using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class IndexPageViewModel
    {
        public BlogInfoViewModel[] Items { get; internal set; }

        public IndexPageViewModel()
        {

        }
    }
}
