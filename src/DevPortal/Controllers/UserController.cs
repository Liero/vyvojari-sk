using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPortal.Web.Data;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.UserViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DevPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using DevPortal.Web.AppCode.Authorization;

namespace DevPortal.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(
            ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string orderBy = null, string search = null)
        {
            var viewmodel = new IndexPageViewModel
            {
                PageNumber = pageNumber
            };

            var query = _applicationDbContext.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.UserName.Contains(search));
            }
            query = query
                .Skip(viewmodel.PagesCount)
                .Take(viewmodel.PageSize);

            viewmodel.Items = await query.ToListAsync();
            viewmodel.TotalCount = await query.CountAsync();

            return View(viewmodel);
        }

        [HttpGet("[Controller]/{username}")]
        public async Task<IActionResult> Detail(string username)
        {
            var user = await _applicationDbContext.Users.Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserName == username);

            ViewBag.RoleNames = await _applicationDbContext.Roles.ToDictionaryAsync(
                r => r.Id, r => r.Name);

            return View(nameof(Detail), user);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("[Controller]/{username}/addrole")]
        public async Task<IActionResult> AddRole(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            var addRoleResult = await _userManager.AddToRoleAsync(user, role);
            return await DisplayRoleResult(addRoleResult, username);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("[Controller]/{username}/removerole")]
        public async Task<IActionResult> RemoveRole(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, role);
            return await DisplayRoleResult(removeRoleResult, role);
        }

        private async Task<IActionResult> DisplayRoleResult(IdentityResult result, string username)
        {
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Detail), new { username });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return await Detail(username);
            }
        }
    }
}