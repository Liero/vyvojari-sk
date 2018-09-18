using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string About { get; set; }

        public string AvatarUrl { get; set; }
        public string ShortDescription { get; set; }

        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    }
}
