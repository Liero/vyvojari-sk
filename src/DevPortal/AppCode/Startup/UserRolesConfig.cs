using DevPortal.Web.AppCode.Authorization;
using DevPortal.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Startup
{
    public class UserRolesConfig
    {
        public static async Task EnsureUserRoles(IServiceProvider applicationServices)
        {
            IServiceScopeFactory scopeFactory = applicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var logger = serviceProvider.GetRequiredService<ILogger<UserRolesConfig>>();

                foreach (var roleName in Roles.AllRoles)
                {
                    var roleCheck = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleCheck)
                    {
                        //create the roles and seed them to the database 
                        var roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                var adminRole = await RoleManager.FindByNameAsync(Roles.Admin);
                bool adminExists = await UserManager.Users.AnyAsync(u => u.UserRoles.Any(r => r.RoleId == adminRole.Id));
                if (!adminExists)
                {
                    const string rootUserName = "root";
                    var userResult = await UserManager.CreateAsync(new ApplicationUser
                    {
                        UserName = rootUserName,
                        Email = "vyvojari@vyvojari.sk"
                    }, "admin");
                    if (!userResult.Succeeded)
                    {
                        logger.LogError("Cannot create default admin user");
                    }
                    var user = await UserManager.FindByNameAsync(rootUserName);
                    await UserManager.AddToRoleAsync(user, Roles.Admin);
                }
            }
        }
    }
}
