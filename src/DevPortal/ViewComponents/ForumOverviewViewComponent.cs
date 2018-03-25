using DevPortal.QueryStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class ForumOverviewViewComponent : ViewComponent
    {
        private readonly DevPortalDbContext _dbContext;

        public ForumOverviewViewComponent(DevPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _dbContext.ForumThreads
                .AsNoTracking()
                .OrderByDescending(b => b.LastPostedBy)
                .Take(10)
                .ToListAsync();

            return View(items);
        }
    }
}
