using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPortal.QueryStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DevPortal.Web.Models.SearchViewModels;
using DevPortal.QueryStack.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly DevPortalDbContext _dbContext;
        private readonly IConfiguration _config;
        const int PageSize = 40;

        public SearchController(DevPortalDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public IActionResult Index(string query, int pageNumber = 1)
        {
            var viewModel = new IndexPageViewModel
            {
                PageNumber = pageNumber
            };


            var dbquery = _dbContext.Contents
                .Where(c => EF.Functions.FreeText(c.Content, query));

            if (!_config.GetValue<bool>("UseFulltextSearch"))
            {
                dbquery = _dbContext.Contents
                    .Include(i => ((ChildContent)i).Root)
                    .Where(c => c.Content.Contains(query));
            }

            viewModel.TotalCount = dbquery.Count();

            var result = dbquery
                .Include(i => ((ChildContent)i).Root)
                .Skip(viewModel.PageNumber - 1)
                .Take(viewModel.PageSize)
                .ToList();

            foreach (var content in result)
            {
                var root = content as GenericContent ?? (content as ChildContent).Root;
                var itemVm = new SearchItem
                {
                    Id = root.Id,
                    Date = content.Created,
                    User = content.CreatedBy,
                    Title = root.Title,
                    ContentType = content.GetType(),
                };
                if (root != content)
                {
                    itemVm.Title = "RE: " + root.Title;
                    itemVm.Fragment = content.Id;
                }
                viewModel.Items.Add(itemVm);
            }
        
            return View(viewModel);
        }
    }
}
