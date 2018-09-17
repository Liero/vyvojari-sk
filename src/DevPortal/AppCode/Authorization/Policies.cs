using DevPortal.Web.AppCode.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Authorization
{
    /// <summary>
    /// Policy is simply a bag of requirement that all must be satisfied.
    /// Requirement is satisfied if at least one RequirementHandler succeeds (and none fails).
    /// 
    /// Requirement can be handler by multiple RequirementHandlers, 
    /// e.g. IsContentOwner is handled by IsContentOwnerHandler and AdminAuthorizationHandler
    /// </summary>
    public static class Policies
    {
        public const string EditPolicy = "EditPolicy";
        public const string DeletePolicy = "DeletePolicy";

        public static void Configure(AuthorizationOptions authorization)
        {
            authorization.AddPolicy(EditPolicy, builder => builder
                .AddRequirements(new IsContentOwner()));

            authorization.AddPolicy(DeletePolicy, builder => builder
                .AddRequirements(new IsContentOwner()));
        }
    }
}
