﻿using DevPortal.QueryStack;
using DevPortal.QueryStack.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class MostUsedTagsViewComponent : ViewComponent
    {
        private readonly DevPortalDbContext _dbContext;

        public MostUsedTagsViewComponent(DevPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Type EntityType { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //todo: group by does not translate to SQL in EF Core 2. Either wait for 2.1, or replace with custom SQL or View
            var tags = await _dbContext.Tags
                .GroupBy(t => t.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(20)
                .ToListAsync();
         
            return View(tags);
        }
    }
}
