using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.HomeViewModels;
using DevPortal.Web.Models.NewsViewModels;
using System.Collections;
using DevPortal.Web.Data;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.AppCode.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DevPortalDbContext _dbContext;

        public HomeController(DevPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                LatestNews = await _dbContext.NewsItems
                    .AsNoTracking()
                    .OrderByDescending(i => i.CreatedBy)
                    .Take(10).ToListAsync(),

                LatestForumThreads = await _dbContext.ForumThreads
                    .AsNoTracking()
                    .OrderByDescending(i => i.Created)
                    .Take(10)
                
                    .ToListAsync(),

                LastestActivity = await _dbContext.Activities
                    .AsNoTracking()
                    .OrderByDescending(i => i.TimeStamp)
                    .Take(100)
                    .ToListAsync()
            };
            return View(viewModel);
        }
    }
}
