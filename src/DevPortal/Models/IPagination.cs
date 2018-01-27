using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models
{
    public interface IPagination
    {
        int PageNumber { get; set; }
        int PagesCount { get; set; }
    }
}
