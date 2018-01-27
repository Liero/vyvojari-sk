using DevPortal.QueryStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class BlogsOverviewViewComponent : ViewComponent
    {
        private readonly DevPortalDbContext _dbContext;

        public BlogsOverviewViewComponent(DevPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _dbContext.Blogs
                .AsNoTracking()
                .OrderByDescending(b => b.Created)
                .Take(10)
                .ToListAsync();

            return View(items);
        }
    }
}
