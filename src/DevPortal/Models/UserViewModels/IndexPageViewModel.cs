using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.UserViewModels
{
    public class IndexPageViewModel: PaginationViewModelBase
    {
        public List<ApplicationUser> Items { get; set; }
    }
}
