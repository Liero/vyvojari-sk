using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DevPortal.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }
    }
}
