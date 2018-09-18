using DevPortal.QueryStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class MostActiveUser
    {
        public string UserName { get; set; }
        public int ContentCount { get; set; }
    }

    public class MostActiveUsersViewComponent : ViewComponent
    {
        private readonly DevPortalDbContext _dbContext;

        public MostActiveUsersViewComponent(DevPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var users = await _dbContext.Contents
                .Where(content => content.Created > DateTime.Today.AddYears(-2))
                .GroupBy(content => content.CreatedBy)
                .Select(group => new MostActiveUser
                {
                    UserName = group.Key,
                    ContentCount = group.Count()
                })
                .OrderByDescending(e => e.ContentCount)
                .Take(10)
                .ToListAsync();

            return View(users);
        }
    }
}
