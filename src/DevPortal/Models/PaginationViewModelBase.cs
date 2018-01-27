using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models
{
    public abstract class PaginationViewModelBase
    {
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int PagesCount => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
