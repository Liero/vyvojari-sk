using DevPortal.QueryStack;
using DevPortal.QueryStack.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Authorization.Requirements
{
    public class IsContentOwner : IAuthorizationRequirement
    {
    }

    public class IsContentOwnerHandler : AuthorizationHandler<IsContentOwner>
    {
        private readonly ILogger _logger;
        private readonly DevPortalDbContext _dbContext;

        public IsContentOwnerHandler(
            ILogger<IsContentOwner> logger,
            DevPortalDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override Task HandleAsync(AuthorizationHandlerContext context)
        {
            return base.HandleAsync(context);
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsContentOwner requirement)
        {
            if (context.Resource == null)
            {
                _logger.LogWarning("Resource is NULL");
                return;
            }
            var entity = await GetEntity(context);
            if (entity == null)
            {
                _logger.LogWarning("Resource {ResourceType} cannot be handled", context.Resource?.GetType().Name ?? "NULL");
                return;
            }

            bool isOwner = String.Equals(
                context.User.Identity.Name,
                entity.CreatedBy,
                StringComparison.InvariantCultureIgnoreCase);

            if (isOwner)
            {
                context.Succeed(requirement);
            }

        }

        async Task<ContentBase> GetEntity(AuthorizationHandlerContext context)
        {
            if (context.Resource is ContentBase content) return content;
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                if (mvcContext.RouteData.Values.TryGetValue("id", out object id))
                {
                    var guid = new Guid(id.ToString());
                    var entity = await _dbContext.Contents.FindAsync(guid);
                    return entity;
                }
            }
            return null;
        }
    }
}
