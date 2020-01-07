using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public static class UserExtensions
    {
        public static bool IdentityNameEquals(this IPrincipal user, string userName)
        {
            return user.Identity.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
