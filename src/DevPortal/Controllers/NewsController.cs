using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Data;

namespace DevPortal.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index(int skip = 1, int take = 20)
        {
            var pageItems = SampleData.Instance.News
                .Skip(skip)
                .Take(take);

            return View(new IndexPageViewModel { Items = pageItems.ToList() });
        }
            
        public IActionResult View(Guid id)
        {
            NewsItemViewModel viewModel = SampleData.Instance.News.First();
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new NewsItemViewModel());
        }

        [HttpPost]
        public IActionResult Create(NewsItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Guid id = /* Save*/ Guid.NewGuid();
            return RedirectToAction(nameof(View), new { id = id });
        }

        public IActionResult Edit(Guid id)
        {
            NewsItemViewModel viewModel = SampleData.Instance.News.First();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(Guid id, NewsItemViewModel viewModel)
        {
            return View();
        }

        public IActionResult Publish(Guid id)
        {
            return RedirectToAction(nameof(View), new { id = id });
        }
    }
}
